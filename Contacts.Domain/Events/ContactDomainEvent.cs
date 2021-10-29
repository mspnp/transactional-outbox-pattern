using System;
using Newtonsoft.Json;

namespace Contacts.Domain.Events
{
    public abstract class ContactDomainEvent : Entity, IEvent
    {
        public Guid ContactId { get; }
        public string Action { get; }
        [JsonProperty] private DateTimeOffset CreatedAt { get; }

        protected ContactDomainEvent(Guid id, Guid contactId, string action)
        {
            Id = id;
            ContactId = contactId;
            Action = action;
            CreatedAt = DateTimeOffset.Now;
        }
    }
}