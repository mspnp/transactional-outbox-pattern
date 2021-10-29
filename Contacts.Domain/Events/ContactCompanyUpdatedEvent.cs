using System;
using Contacts.Domain.ValueObjects;

namespace Contacts.Domain.Events
{
    public class ContactCompanyUpdatedEvent : ContactDomainEvent
    {
        public Company Company { get; }

        public ContactCompanyUpdatedEvent(Guid contactId, Company company) : base(Guid.NewGuid(), contactId,
            nameof(ContactCompanyUpdatedEvent))
        {
            Company = company;
        }
    }
}