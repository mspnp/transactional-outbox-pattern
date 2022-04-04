using System.Collections.Generic;
using Contacts.API.Middlewares;
using Contacts.Application.Behaviors;
using Contacts.Application.Commands;
using Contacts.Application.Commands.Validators;
using Contacts.Application.Mapper;
using Contacts.Domain;
using Contacts.Domain.Events;
using Contacts.Infrastructure;
using Contacts.Infrastructure.Context;
using Contacts.Infrastructure.EventHandlers;
using Contacts.Infrastructure.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Contacts.API;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var cOpts = new CosmosClientOptions
        {
            SerializerOptions = new CosmosSerializationOptions()
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                IgnoreNullValues = true
            }
        };
        var containers = new List<(string, string)>
        {
            (Configuration.GetSection("Cosmos")["Db"], Configuration.GetSection("Cosmos")["Container"])
        };
        var cosmosClient = CosmosClient.CreateAndInitializeAsync(Configuration.GetSection("Cosmos")["Url"],
            Configuration.GetSection("Cosmos")["Key"], containers, cOpts).Result;

        var container = cosmosClient.GetContainer(Configuration.GetSection("Cosmos")["Db"],
            Configuration.GetSection("Cosmos")["Container"]);

        ContactPartitionKeyProvider partitionKeyProvider = new();

        services.AddAutoMapper(typeof(ContactsProfile));
        services.AddValidatorsFromAssembly(typeof(CreateContactCommandValidator).Assembly);

        services.AddSingleton(container)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
            .AddSingleton<IContactPartitionKeyProvider>(partitionKeyProvider)
            .AddScoped<IContainerContext, CosmosContainerContext>()
            .AddScoped<IEventRepository, EventRepository>()
            .AddScoped<IContactRepository, ContactRepository>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddMediatR(typeof(Contact), typeof(ContactCompanyUpdatedHandler), typeof(CreateContactCommand));

        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Contacts.API", Version = "v1" });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
            
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contacts.API v1"));

        app.UseMiddleware<ExceptionHandlerMiddleware>();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}