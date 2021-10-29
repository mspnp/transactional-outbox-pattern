using System.Collections.Generic;

namespace Contacts.Domain.Events
{
    public interface IEventEmitter<T> where T : IEvent
    {
        public void AddEvent(T domainEvent);
        public void RemoveEvent(T domainEvent);
        public void RemoveAllEvents();
        public IReadOnlyList<T> DomainEvents { get; }
    }
}