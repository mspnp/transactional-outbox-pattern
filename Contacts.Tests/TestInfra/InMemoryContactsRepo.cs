using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contacts.Domain;
using Contacts.Infrastructure.Context;
using Newtonsoft.Json.Linq;

namespace Contacts.Tests.TestInfra
{
    public class InMemoryContactsRepo : IContactRepository
    {
        private readonly IContainerContext _context;
        private readonly IContactPartitionKeyProvider _partitionKeyProvider;
        private const string CONTACT_TYPE = "contact";
        private string _jsonObject;

        public InMemoryContactsRepo(IContainerContext context, IContactPartitionKeyProvider partitionKeyProvider)
        {
            _context = context;
            _partitionKeyProvider = partitionKeyProvider;
            _jsonObject = @"
                {
                    ""name"": {
                        ""firstName"": ""John"",
                        ""lastName"": ""Doe""
                    },
                    ""description"": ""This is a description"",
                    ""email"": ""jd@example.com"",
                    ""company"": {
                        ""companyName"": ""Example"",
                        ""street"": ""Street"",
                        ""houseNumber"": ""1a"",
                        ""postalCode"": ""092821"",
                        ""city"": ""Palo Alto"",
                        ""country"": ""US""
                    },
                    ""id"": ""0001f5bb-d4a9-48ba-af80-3db52bfd7411""
                }";
        }


        public void Create(Contact contact)
        {
            var o = new DataObject<Contact>(contact.Id.ToString(), _partitionKeyProvider.GetPartitionKey(contact),
                CONTACT_TYPE,
                contact, "123", -1, EntityState.Created);

            _context.Add(o);
        }

        public Task<(Contact, string)> ReadAsync(Guid id, string etag)
        {
            var jObj = JObject.Parse(_jsonObject);
            jObj["id"] = id.ToString();
            var c = jObj.ToObject<Contact>();
            return Task.FromResult((c, "123"));
        }

        public async Task DeleteAsync(Guid id, string etag)
        {
            var (c, etagRes) = await ReadAsync(id, etag);
            c.SetDeleted();

            var o = new DataObject<Contact>(c.Id.ToString(), _partitionKeyProvider.GetPartitionKey(c),
                CONTACT_TYPE,
                c, etagRes, -1, EntityState.Deleted);
            _context.Add(o);
        }

        public Task<(List<(Contact, string)>, bool, string)> ReadAllAsync(int pageSize, string continuationToken)
        {
            var jObj = JObject.Parse(_jsonObject);
            var c = jObj.ToObject<Contact>();
            return Task.FromResult((new List<(Contact, string)>() {(c, "123")}, false, ""));
        }

        public void Update(Contact contact, string etag)
        {
            var o = new DataObject<Contact>(contact.Id.ToString(), _partitionKeyProvider.GetPartitionKey(contact),
                CONTACT_TYPE,
                contact, "123", -1, EntityState.Updated);

            _context.Add(o);
        }
    }
}