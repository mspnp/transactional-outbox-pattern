using Contacts.Domain.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
namespace Contacts.Infrastructure.EventHandlers;

public class ContactNameUpdatedHandler : INotificationHandler<ContactNameUpdatedEvent>
{
    private IEventRepository EventRepository { get; }

    public ContactNameUpdatedHandler(IEventRepository eventRepo)
    {
        EventRepository = eventRepo;
    }


    public Task Handle(ContactNameUpdatedEvent notification, CancellationToken cancellationToken)
    {
        EventRepository.Create(notification);
        return Task.CompletedTask;
    }
}