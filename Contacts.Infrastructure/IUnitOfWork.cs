using System.Collections.Generic;
using Contacts.Domain;
using System.Threading;
using System.Threading.Tasks;
using Contacts.Infrastructure.Context;

namespace Contacts.Infrastructure;

public interface IUnitOfWork
{
    IContactRepository ContactsRepo { get; }
    Task<List<IDataObject<Entity>>> CommitAsync(CancellationToken cancellationToken = default);
}