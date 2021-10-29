using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contacts.Domain;
using Contacts.Domain.Events;
using Contacts.Infrastructure.Context;
using MediatR;
using Microsoft.Azure.Cosmos;

namespace Contacts.Tests.TestInfra
{
    public class TestContainerContext : IContainerContext
    {
        private readonly IMediator _mediator;

        public TestContainerContext(IMediator mediator)
        {
            _mediator = mediator;
        }
        public void Add(IDataObject<Entity> entity)
        {
            if (DataObjects.FindIndex(0,
                o => o.Id == entity.Id && o.PartitionKey == entity.PartitionKey) == -1)
                DataObjects.Add(entity);
        }
        
        private void RaiseDomainEvents(List<IDataObject<Entity>> dObjs)
        {
            var eventEmitters = new List<IEventEmitter<IEvent>>();

            // Get all EventEmitters
            foreach (var o in dObjs)
            {
                if (o.Data is IEventEmitter<IEvent> ee)
                    eventEmitters.Add(ee);
            }

            // Raise Events
            if (eventEmitters.Count > 0)
            {
                foreach (var evt in eventEmitters.SelectMany(eventEmitter => eventEmitter.DomainEvents))
                    _mediator.Publish(evt);
            }
        }

        public Task<List<IDataObject<Entity>>> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if (DataObjects.Count == 0)
                return null;

            RaiseDomainEvents(DataObjects);
            var res = new List<IDataObject<Entity>>(DataObjects);
            DataObjects.Clear();
            return Task.FromResult<List<IDataObject<Entity>>>(res);
        }

        public Container Container { get; } 

        public List<IDataObject<Entity>> DataObjects { get; } = new();

        public void Reset()
        {
            DataObjects.Clear();
        }
    }
}