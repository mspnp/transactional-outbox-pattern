using System.Collections.Generic;
using Contacts.Domain;
using Contacts.Infrastructure.Context;
using System.Threading;
using System.Threading.Tasks;

namespace Contacts.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IContainerContext _context;
        public IContactRepository ContactsRepo { get; }

        public UnitOfWork(IContainerContext ctx, IContactRepository cRepo)
        {
            _context = ctx;
            ContactsRepo = cRepo;
        }

        public Task<List<IDataObject<Entity>>> CommitAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}