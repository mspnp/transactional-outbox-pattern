using Contacts.Domain.Events;
using Contacts.Infrastructure.Context;

namespace Contacts.Tests.TestInfra
{
    public class InMemoryEventRepo : IEventRepository
    {
        public IContainerContext Context { get; }
        private const string EVENT_TYPE = "event";

        public InMemoryEventRepo(IContainerContext ctx)
        {
            Context = ctx;
        }

        public void Create(ContactDomainEvent e)
        {
            var o = new DataObject<ContactDomainEvent>(e.Id.ToString(), e.ContactId.ToString(), EVENT_TYPE, e, null,
                120, EntityState.Created);

            Context.Add(o);
        }
    }
}