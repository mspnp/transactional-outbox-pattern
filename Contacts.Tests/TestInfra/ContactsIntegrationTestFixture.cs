using System;
using System.Collections.Generic;
using Contacts.Domain;
using Contacts.Domain.Events;
using Contacts.Infrastructure;
using Contacts.Infrastructure.Context;
using Contacts.Infrastructure.EventHandlers;
using Contacts.Infrastructure.Repositories;
using MediatR;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Contacts.Tests.TestInfra;

public class ContactsIntegrationTestFixture : IDisposable
{
    public ServiceProvider Provider { get; }
    public Guid CurrentId { get; }
    private CosmosClient cosmosClient;
    private Container container;

    public ContactsIntegrationTestFixture()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddEnvironmentVariables()
            .AddJsonFile("test_appsettings.json")
            .Build();

        CurrentId = Guid.NewGuid();

        var cOpts = new CosmosClientOptions();
        cOpts.SerializerOptions = new CosmosSerializationOptions()
        {
            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
            IgnoreNullValues = true
        };
        var containers = new List<(string, string)> 
        {
            (configuration.GetSection("Cosmos")["Db"], configuration.GetSection("Cosmos")["Container"])
        };
        cosmosClient = CosmosClient.CreateAndInitializeAsync(configuration.GetSection("Cosmos")["Url"],
            configuration.GetSection("Cosmos")["Key"], containers, cOpts).Result;
        container = cosmosClient.GetContainer(configuration.GetSection("Cosmos")["Db"],
            configuration.GetSection("Cosmos")["Container"]);

        var cx = cosmosClient.ReadAccountAsync().Result;

        ContactPartitionKeyProvider pkprovider = new();

        Provider = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .AddSingleton(container)
            .AddSingleton<IContactPartitionKeyProvider>(pkprovider)
            .AddScoped<IContainerContext, CosmosContainerContext>()
            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IContactRepository, ContactRepository>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddMediatR(typeof(Contact), typeof(ContactCompanyUpdatedHandler))
            .BuildServiceProvider();
    }

    public async void Dispose()
    {
        await container.DeleteItemAsync<DataObject<Contact>>(CurrentId.ToString(),
            new PartitionKey(CurrentId.ToString()));
    }
}