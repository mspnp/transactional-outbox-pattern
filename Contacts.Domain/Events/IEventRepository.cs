namespace Contacts.Domain.Events
{
    public interface IEventRepository
    {
        public void Create(ContactDomainEvent e);
    }
}
