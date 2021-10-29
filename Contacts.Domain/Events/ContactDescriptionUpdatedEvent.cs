using System;

namespace Contacts.Domain.Events
{
    public class ContactDescriptionUpdatedEvent : ContactDomainEvent
    {
        public string Description { get; }

        public ContactDescriptionUpdatedEvent(Guid contactId, string description) : base(Guid.NewGuid(), contactId,
            nameof(ContactDescriptionUpdatedEvent))
        {
            Description = description;
        }
    }
}