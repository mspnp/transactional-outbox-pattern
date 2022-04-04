using Contacts.Domain.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
namespace Contacts.Infrastructure.EventHandlers;

public class ContactDescriptionUpdatedHandler : INotificationHandler<ContactDescriptionUpdatedEvent>
{
    private IEventRepository EventRepository { get; }

    public ContactDescriptionUpdatedHandler(IEventRepository eventRepo)
    {
        EventRepository = eventRepo;
    }


    public Task Handle(ContactDescriptionUpdatedEvent notification, CancellationToken cancellationToken)
    {
        EventRepository.Create(notification);
        return Task.CompletedTask;
    }
}