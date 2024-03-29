﻿using System;
using Contacts.Application.Commands;
using Contacts.Domain;
using Contacts.Domain.Events;
using Contacts.Infrastructure;
using Contacts.Infrastructure.Context;
using Contacts.Infrastructure.EventHandlers;
using Contacts.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Contacts.Tests.TestInfra;

public class ContactsCommandsTestFixture : IDisposable
{
    public ServiceProvider Provider { get; }
    public Guid CurrentId { get; }


    public ContactsCommandsTestFixture()
    {
        ContactPartitionKeyProvider partitionKeyProvider = new();
        CurrentId = Guid.NewGuid();

        Provider = new ServiceCollection()
            .AddSingleton<IContactPartitionKeyProvider>(partitionKeyProvider)
            .AddScoped<IContainerContext, TestContainerContext>()
            .AddScoped<IEventRepository, InMemoryEventRepo>()
            .AddScoped<IContactRepository, InMemoryContactsRepo>()
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Contact).Assembly,
                typeof(CreateContactCommand).Assembly,
                typeof(ContactCreatedHandler).Assembly))
            .BuildServiceProvider();
    }

    public void Dispose()
    {
    }
}