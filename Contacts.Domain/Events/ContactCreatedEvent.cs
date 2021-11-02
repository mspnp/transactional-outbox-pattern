using System;

namespace Contacts.Domain.Events
{
    public class ContactCreatedEvent : ContactDomainEvent
    {
        
        public Contact Contact { get; }

        public ContactCreatedEvent(Guid contactId, Contact contact) : base(Guid.NewGuid(), contactId,
            nameof(ContactCreatedEvent))
        {
            Contact = contact;
        }
    }
}