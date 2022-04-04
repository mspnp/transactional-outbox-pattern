using Contacts.Domain.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
namespace Contacts.Infrastructure.EventHandlers;

public class ContactCreatedHandler : INotificationHandler<ContactCreatedEvent>
{
    private IEventRepository EventRepository { get; }

    public ContactCreatedHandler(IEventRepository eventRepo)
    {
        EventRepository = eventRepo;
    }


    public Task Handle(ContactCreatedEvent notification, CancellationToken cancellationToken)
    {
        EventRepository.Create(notification);
        return Task.CompletedTask;
    }
}