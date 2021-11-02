using System;
using Contacts.Domain.ValueObjects;

namespace Contacts.Domain.Events
{
    public class ContactNameUpdatedEvent : ContactDomainEvent
    {
        public Name Name { get; }

        public ContactNameUpdatedEvent(Guid contactId, Name contactName) : base(Guid.NewGuid(), contactId,
            nameof(ContactNameUpdatedEvent))
        {
            Name = contactName;
        }
    }
}