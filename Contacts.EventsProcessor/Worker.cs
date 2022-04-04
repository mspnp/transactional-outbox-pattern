using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Contacts.EventsProcessor;

public class Worker : BackgroundService
{
    private const string EVENT_TYPE = "domainEvent";
    private readonly IConfiguration _configuration;
    private readonly Container _container;
    private readonly CosmosClient _cosmosClient;
    private readonly Container _leaseContainer;
    private readonly ILogger<Worker> _logger;
    private readonly ServiceBusClient _sbClient;
    private readonly ServiceBusSender _topicSender;
    private ChangeFeedProcessor _cfp;

    public Worker(ILogger<Worker> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;

        _sbClient = new ServiceBusClient(_configuration.GetSection("ServiceBus")["ConnectionString"]);
        _topicSender = _sbClient.CreateSender(_configuration.GetSection("ServiceBus")["TopicName"]);

        var cOpts = new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                IgnoreNullValues = true
            }
        };
        var containers = new List<(string, string)>
        {
            (_configuration.GetSection("Cosmos")["Db"], _configuration.GetSection("Cosmos")["Container"])
        };
        _cosmosClient = CosmosClient.CreateAndInitializeAsync(_configuration.GetSection("Cosmos")["Url"],
            _configuration.GetSection("Cosmos")["Key"], containers, cOpts).Result;

        _container = _cosmosClient.GetContainer(_configuration.GetSection("Cosmos")["Db"],
            _configuration.GetSection("Cosmos")["Container"]);
        _leaseContainer = _cosmosClient.GetDatabase(_configuration.GetSection("Cosmos")["Db"])
            .CreateContainerIfNotExistsAsync(_configuration.GetSection("Cosmos")["LeaseContainer"],
                "/id").Result;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Events Processor (Contacts) running at: {time}.",
            DateTimeOffset.UtcNow);
        _cfp = await StartChangeFeedProcessorAsync();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Events Processor (Contacts) at: {time}", DateTimeOffset.UtcNow);
        await _cfp.StopAsync();
        await _topicSender.DisposeAsync();
        await _sbClient.DisposeAsync();
        _logger.LogInformation("Events Processor (Contacts) stopped. Bye bye!");
    }

    private async Task<ChangeFeedProcessor> StartChangeFeedProcessorAsync()
    {
        var changeFeedProcessor = _container
            .GetChangeFeedProcessorBuilder<ExpandoObject>(
                _configuration.GetSection("Cosmos")["ProcessorName"],
                HandleChangesAsync)
            .WithInstanceName(Environment.MachineName)
            .WithLeaseContainer(_leaseContainer)
            .WithMaxItems(25)
            .WithStartTime(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .WithPollInterval(TimeSpan.FromSeconds(3))
            .Build();

        _logger.LogInformation("Starting Change Feed Processor...");
        await changeFeedProcessor.StartAsync();
        _logger.LogInformation("Change Feed Processor started.  Waiting for new messages to arrive.");
        return changeFeedProcessor;
    }

    private async Task HandleChangesAsync(IReadOnlyCollection<ExpandoObject> changes,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Received {changes.Count} document(s).");
        var eventsCount = 0;

        Dictionary<string, List<ServiceBusMessage>> partitionedMessages = new();

        foreach (var document in changes as dynamic)
        {
            if (!((IDictionary<string, object>)document).ContainsKey("type") ||
                !((IDictionary<string, object>)document).ContainsKey("data")) continue; // unknown doc type

            if (document.type == EVENT_TYPE)
            {
                string json = JsonConvert.SerializeObject(document.data);
                var sbMessage = new ServiceBusMessage(json)
                {
                    ContentType = "application/json",
                    Subject = document.data.action,
                    MessageId = document.id,
                    PartitionKey = document.partitionKey,
                    SessionId = document.partitionKey
                };

                // Create message batch per partitionKey
                if (partitionedMessages.ContainsKey(document.partitionKey))
                {
                    partitionedMessages[sbMessage.PartitionKey].Add(sbMessage);
                }
                else
                {
                    partitionedMessages[sbMessage.PartitionKey] = new List<ServiceBusMessage> { sbMessage };
                }

                eventsCount++;
            }
        }

        if (partitionedMessages.Count > 0)
        {
            _logger.LogInformation(
                $"Processing {eventsCount} event(s) in {partitionedMessages.Count} partition(s).");

            // Loop over each partition
            foreach (var partition in partitionedMessages)
            {
                // Create batch for partition
                using var messageBatch =
                    await _topicSender.CreateMessageBatchAsync(cancellationToken);
                foreach (var msg in partition.Value)
                    if (!messageBatch.TryAddMessage(msg))
                        throw new Exception();

                _logger.LogInformation(
                    $"Sending {messageBatch.Count} event(s) to Service Bus. PartitionId: {partition.Key}");

                try
                {
                    await _topicSender.SendMessagesAsync(messageBatch, cancellationToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    throw;
                }
            }
        }
        else
        {
            _logger.LogInformation("No event documents in change feed batch. Waiting for new messages to arrive.");
        }
    }
}