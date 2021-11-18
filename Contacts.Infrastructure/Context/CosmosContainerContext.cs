using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Contacts.Domain;
using Contacts.Domain.Events;
using Contacts.Infrastructure.Exceptions;
using MediatR;
using Microsoft.Azure.Cosmos;

namespace Contacts.Infrastructure.Context
{
    public class CosmosContainerContext : IContainerContext
    {
        private readonly IMediator _mediator;

        public CosmosContainerContext(Container container, IMediator mediator)
        {
            Container = container;
            _mediator = mediator;
        }

        // for multi-threading, this should be migrated to one of the 
        // concurrent collections of C# 
        public List<IDataObject<Entity>> DataObjects { get; } = new();

        public void Reset()
        {
            DataObjects.Clear();
        }

        public Container Container { get; }

        public void Add(IDataObject<Entity> entity)
        {
            if (DataObjects.FindIndex(0,
                o => o.Id == entity.Id && o.PartitionKey == entity.PartitionKey) == -1)
                DataObjects.Add(entity);
        }

        public async Task<List<IDataObject<Entity>>> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            RaiseDomainEvents(DataObjects);

            switch (DataObjects.Count)
            {
                case 1:
                    {
                        var result = await SaveSingleAsync(DataObjects[0], cancellationToken);
                        return result;
                    }
                case > 1:
                    {
                        var result = await SaveInTransactionalBatchAsync(cancellationToken);
                        return result;
                    }
                default:
                    return new List<IDataObject<Entity>>();
            }
        }

        private async Task<List<IDataObject<Entity>>> SaveInTransactionalBatchAsync(
            CancellationToken cancellationToken)
        {
            if (DataObjects.Count > 0)
            {
                var pk = new PartitionKey(DataObjects[0].PartitionKey);
                var tb = Container.CreateTransactionalBatch(pk);
                DataObjects.ForEach(o =>
                {
                    TransactionalBatchItemRequestOptions tro = null;

                    if (!string.IsNullOrWhiteSpace(o.Etag))
                        tro = new TransactionalBatchItemRequestOptions { IfMatchEtag = o.Etag };

                    switch (o.State)
                    {
                        case EntityState.Created:
                            tb.CreateItem(o);
                            break;
                        case EntityState.Updated or EntityState.Deleted:
                            tb.ReplaceItem(o.Id, o, tro);
                            break;
                    }
                });

                var tbResult = await tb.ExecuteAsync(cancellationToken);

                if (!tbResult.IsSuccessStatusCode)
                    for (var i = 0; i < DataObjects.Count; i++)
                        if (tbResult[i].StatusCode != HttpStatusCode.FailedDependency)
                        {
                            // Not recoverable - clear context
                            DataObjects.Clear();
                            throw EvaluateCosmosError(tbResult[i].StatusCode);
                        }

                for (var i = 0; i < DataObjects.Count; i++)
                    DataObjects[i].Etag = tbResult[i].ETag;
            }

            var result = new List<IDataObject<Entity>>(DataObjects); // return copy of list as result

            // work has been successfully done - reset DataObjects list
            DataObjects.Clear();
            return result;
        }

        private async Task<List<IDataObject<Entity>>> SaveSingleAsync(IDataObject<Entity> dObj,
            CancellationToken cancellationToken = default)
        {
            var reqOptions = new ItemRequestOptions
            {
                EnableContentResponseOnWrite = false
            };

            if (!string.IsNullOrWhiteSpace(dObj.Etag)) reqOptions.IfMatchEtag = dObj.Etag;

            var pk = new PartitionKey(dObj.PartitionKey);

            try
            {
                ItemResponse<IDataObject<Entity>> response;

                switch (dObj.State)
                {
                    case EntityState.Created:
                        response = await Container.CreateItemAsync(dObj, pk, reqOptions, cancellationToken);
                        break;
                    case EntityState.Updated:
                    case EntityState.Deleted:
                        response = await Container.ReplaceItemAsync(dObj, dObj.Id, pk, reqOptions, cancellationToken);
                        break;
                    default:
                        DataObjects.Clear();
                        return new List<IDataObject<Entity>>();
                }

                dObj.Etag = response.ETag;
                var result = new List<IDataObject<Entity>>(1) { dObj };

                // work has been successfully done - reset DataObjects list
                DataObjects.Clear();
                return result;
            }
            catch (CosmosException e)
            {
                // Not recoverable - clear context
                DataObjects.Clear();
                throw EvaluateCosmosError(e, Guid.Parse(dObj.Id), dObj.Etag);
            }
        }

        private void RaiseDomainEvents(List<IDataObject<Entity>> dObjs)
        {
            var eventEmitters = new List<IEventEmitter<IEvent>>();

            // Get all EventEmitters
            foreach (var o in dObjs)
                if (o.Data is IEventEmitter<IEvent> ee)
                    eventEmitters.Add(ee);

            // Raise Events
            if (eventEmitters.Count <= 0) return;
            foreach (var evt in eventEmitters.SelectMany(eventEmitter => eventEmitter.DomainEvents))
                _mediator.Publish(evt);
        }

        private Exception EvaluateCosmosError(CosmosException error, Guid? id = null, string etag = null)
        {
            return EvaluateCosmosError(error.StatusCode, id, etag);
        }

        private Exception EvaluateCosmosError(HttpStatusCode statusCode, Guid? id = null, string etag = null)
        {
            return statusCode switch
            {
                HttpStatusCode.NotFound => new DomainObjectNotFoundException(
                    $"Domain object not found for Id: {(id != null ? id.Value : string.Empty)} / ETag: {etag}"),
                HttpStatusCode.NotModified => new DomainObjectNotModifiedException(
                    $"Domain object not modified. Id: {(id != null ? id.Value : string.Empty)} / ETag: {etag}"),
                HttpStatusCode.Conflict => new DomainObjectConflictException(
                    $"Domain object conflict detected. Id: {(id != null ? id.Value : string.Empty)} / ETag: {etag}"),
                HttpStatusCode.PreconditionFailed => new DomainObjectPreconditionFailedException(
                    $"Domain object mid-air collision detected. Id: {(id != null ? id.Value : string.Empty)} / ETag: {etag}"),
                HttpStatusCode.TooManyRequests => new DomainObjectTooManyRequestsException(
                    "Too many requests occurred. Try again later)"),
                _ => new Exception("Cosmos Exception")
            };
        }
    }
}