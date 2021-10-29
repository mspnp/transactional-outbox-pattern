using System;

namespace Contacts.Domain.Events
{
    public class ContactDeletedEvent : ContactDomainEvent
    {
        public ContactDeletedEvent(Guid contactId) : base(Guid.NewGuid(), contactId,
            nameof(ContactDeletedEvent)) { }
    }
}