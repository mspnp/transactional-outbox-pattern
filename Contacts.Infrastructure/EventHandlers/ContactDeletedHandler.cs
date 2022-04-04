using Contacts.Domain.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
namespace Contacts.Infrastructure.EventHandlers;

public class ContactDeletedHandler : INotificationHandler<ContactDeletedEvent>
{
    private IEventRepository EventRepository { get; }

    public ContactDeletedHandler(IEventRepository eventRepo)
    {
        EventRepository = eventRepo;
    }


    public Task Handle(ContactDeletedEvent notification, CancellationToken cancellationToken)
    {
        EventRepository.Create(notification);
        return Task.CompletedTask;
    }
}