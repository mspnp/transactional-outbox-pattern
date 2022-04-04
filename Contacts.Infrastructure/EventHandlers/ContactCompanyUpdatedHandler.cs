using Contacts.Domain.Events;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
namespace Contacts.Infrastructure.EventHandlers;

public class ContactCompanyUpdatedHandler : INotificationHandler<ContactCompanyUpdatedEvent>
{
    private IEventRepository EventRepository { get; }

    public ContactCompanyUpdatedHandler(IEventRepository eventRepo)
    {
        EventRepository = eventRepo;
    }


    public Task Handle(ContactCompanyUpdatedEvent notification, CancellationToken cancellationToken)
    {
        EventRepository.Create(notification);
        return Task.CompletedTask;
    }
}