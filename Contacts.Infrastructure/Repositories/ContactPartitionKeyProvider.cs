using Contacts.Domain;

namespace Contacts.Infrastructure.Repositories
{
    public class ContactPartitionKeyProvider : IContactPartitionKeyProvider
    {
        public string GetPartitionKey(Contact contact)
        {
            return $"{contact.Id}";
        }

        public string GetPartitionKey(string id)
        {
            return id;
        }
    }
}
