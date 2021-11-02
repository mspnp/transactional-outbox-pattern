using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contacts.Domain
{
    public interface IContactRepository
    {
        public void Create(Contact contact);
        public Task<(Contact, string)> ReadAsync(Guid id, string etag);
        public Task DeleteAsync(Guid id, string etag);
        public Task<(List<(Contact, string)>, bool, string)> ReadAllAsync(int pageSize, string continuationToken);
        public void Update(Contact contact, string etag);
    }
}