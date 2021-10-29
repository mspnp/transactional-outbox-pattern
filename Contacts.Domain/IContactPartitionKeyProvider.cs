namespace Contacts.Domain
{
    public interface IContactPartitionKeyProvider
    {
        public string GetPartitionKey(Contact contact);
        public string GetPartitionKey(string id);
    }
}
