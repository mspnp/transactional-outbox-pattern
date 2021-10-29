using Contacts.Domain.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
namespace Contacts.Infrastructure.EventHandlers
{
    public class ContactEmailUpdatedHandler : INotificationHandler<ContactEmailUpdatedEvent>
    {
        private IEventRepository EventRepository { get; }

        public ContactEmailUpdatedHandler(IEventRepository eventRepo)
        {
            EventRepository = eventRepo;
        }


        public Task Handle(ContactEmailUpdatedEvent notification, CancellationToken cancellationToken)
        {
            EventRepository.Create(notification);
            return Task.CompletedTask;
        }
    }
}
