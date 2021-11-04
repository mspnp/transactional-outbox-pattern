using System;
using System.Collections.Generic;
using Contacts.Domain.Events;
using Newtonsoft.Json;

namespace Contacts.Domain
{
    public abstract class DomainEntity : Entity, IEventEmitter<IEvent>
    {
        [JsonProperty] public DateTimeOffset CreatedAt { get; protected set; }
        [JsonProperty] public DateTimeOffset? ModifiedAt { get; protected set; }
        [JsonProperty] public DateTimeOffset? DeletedAt { get; protected set; }

        [JsonProperty] public bool Deleted { get; protected set; }

        [JsonIgnore] protected bool IsNew { get; init; }
        
        // for multi-threading, this should be migrated to one of the 
        // concurrent collections of C# 
        private readonly List<IEvent> _events = new();

        [JsonIgnore] public IReadOnlyList<IEvent> DomainEvents => _events.AsReadOnly();

        public virtual void AddEvent(IEvent domainEvent)
        {
            var i = _events.FindIndex(0, e => e.Action == domainEvent.Action);
            if (i < 0)
            {
                _events.Add(domainEvent);
            }
            else
            {
                _events.RemoveAt(i);
                _events.Insert(i, domainEvent);
            }
        }

        public virtual void RemoveEvent(IEvent domainEvent)
        {
            _events.Remove(domainEvent);
        }

        public virtual void RemoveAllEvents()
        {
            _events.Clear();
        }
    }
}