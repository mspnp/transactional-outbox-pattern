using Contacts.Domain.Events;
using Contacts.Infrastructure.Context;
using Microsoft.Extensions.Configuration;

namespace Contacts.Infrastructure.Repositories;

public class EventRepository : IEventRepository
{
    private readonly IConfiguration _cfg;
    private IContainerContext Context { get; }

    private const string EVENT_TYPE = "domainEvent";
    private readonly int DEFAULT_TTL;

    public EventRepository(IContainerContext ctx, IConfiguration cfg)
    {
        _cfg = cfg;
        DEFAULT_TTL = _cfg.GetSection("Events")?["Ttl"] == null
            ? 120
            : int.Parse(_cfg.GetSection("Events")?["Ttl"]);
        Context = ctx;
    }

    public void Create(ContactDomainEvent e)
    {
        var o = new DataObject<ContactDomainEvent>(e.Id.ToString(), e.ContactId.ToString(), EVENT_TYPE, e, null,
            DEFAULT_TTL, EntityState.Created);
        Context.Add(o);
    }
}