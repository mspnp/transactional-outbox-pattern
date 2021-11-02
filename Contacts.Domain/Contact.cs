using Contacts.Domain.Events;
using Contacts.Domain.ValueObjects;
using System;
using Newtonsoft.Json;

namespace Contacts.Domain
{
    public class Contact : DomainEntity, IAggregateRoot
    {
        [JsonProperty] public Name Name { get; private set; }
        [JsonProperty] public string Description { get; private set; }
        [JsonProperty] public string Email { get; private set; }
        [JsonProperty] public Company Company { get; private set; }

        private Contact()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTimeOffset.UtcNow;
        }

        private Contact(Guid id)
        {
            Id = id;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public static Contact CreateNew()
        {
            var c = new Contact { IsNew = true };

            // Raise Event
            c.AddEvent(new ContactCreatedEvent(c.Id, c));
            return c;
        }

        public static Contact CreateNew(Guid id)
        {
            var c = new Contact(id) { IsNew = true };

            // Raise Event
            c.AddEvent(new ContactCreatedEvent(c.Id, c));
            return c;
        }

        public void SetName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("FirstName or LastName may not be empty");
            }

            Name = new Name(firstName, lastName);

            if (IsNew) return;

            AddEvent(new ContactNameUpdatedEvent(Id, Name));
            ModifiedAt = DateTimeOffset.UtcNow;
        }

        public void SetDescription(string description)
        {
            Description = description;

            if (IsNew) return;

            AddEvent(new ContactDescriptionUpdatedEvent(Id, Description));
            ModifiedAt = DateTimeOffset.UtcNow;
        }

        public void SetEmail(string email)
        {
            Email = email;

            if (IsNew) return;

            AddEvent(new ContactEmailUpdatedEvent(Id, Email));
            ModifiedAt = DateTimeOffset.UtcNow;
        }

        public void SetCompany(string companyName, string street, string houseNumber, string postalCode, string city,
            string country)
        {
            if (string.IsNullOrWhiteSpace(companyName))
            {
                throw new ArgumentException("Company Name may not be empty");
            }

            Company = new Company(companyName, street, houseNumber, postalCode, city, country);

            if (IsNew) return;
            
            AddEvent(new ContactCompanyUpdatedEvent(Id, Company));
            ModifiedAt = DateTimeOffset.UtcNow;
        }

        public void SetDeleted()
        {
            if (IsNew) return;
            
            AddEvent(new ContactDeletedEvent(Id));
            DeletedAt = DateTimeOffset.UtcNow;
            Deleted = true;
        }
    }
}