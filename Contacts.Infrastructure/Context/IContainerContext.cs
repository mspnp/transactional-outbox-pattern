using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Contacts.Domain;
using Microsoft.Azure.Cosmos;

namespace Contacts.Infrastructure.Context;

public interface IContainerContext
{
    public Container Container { get; }
    public List<IDataObject<Entity>> DataObjects { get; }
    public void Add(IDataObject<Entity> entity);
    public Task<List<IDataObject<Entity>>> SaveChangesAsync(CancellationToken cancellationToken = default);
    public void Reset();
}