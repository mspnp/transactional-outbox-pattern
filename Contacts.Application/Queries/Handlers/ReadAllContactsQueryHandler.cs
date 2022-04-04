using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Contacts.Application.Queries.Responses;
using Contacts.Infrastructure;
using MediatR;

namespace Contacts.Application.Queries.Handlers;

public class
    ReadAllContactsQueryHandler : IRequestHandler<ReadAllContactsQuery,
        ReadAllContactsQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReadAllContactsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReadAllContactsQueryResponse> Handle(ReadAllContactsQuery request,
        CancellationToken cancellationToken)
    {
        var (list, hasMore, cToken) =
            await _unitOfWork.ContactsRepo.ReadAllAsync(request.PageSize, request.ContinuationToken);

        if (list.Count > 0)
        {
            var outList = new List<ContactsListModel>();
            foreach (var (item, etag) in list)
            {
                outList.Add(new ContactsListModel(item.Id.ToString(), etag, item.Email,
                    $"{item.Name.FirstName} {item.Name.LastName}", item.Company.CompanyName));
            }

            return new ReadAllContactsQueryResponse(hasMore, cToken, outList);
        }
        else
        {
            return new ReadAllContactsQueryResponse(hasMore, cToken, new List<ContactsListModel>());
        }
    }
}