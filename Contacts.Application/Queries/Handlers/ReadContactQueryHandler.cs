using System.Threading;
using System.Threading.Tasks;
using Contacts.Application.Queries.Responses;
using Contacts.Infrastructure;
using MediatR;

namespace Contacts.Application.Queries.Handlers;

public class
    ReadContactQueryHandler : IRequestHandler<ReadContactQuery,
        ReadContactQueryResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public ReadContactQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ReadContactQueryResponse> Handle(ReadContactQuery request,
        CancellationToken cancellationToken)
    {
        var (contact, etag) = await _unitOfWork.ContactsRepo.ReadAsync(request.Id, request.Etag);

        return new ReadContactQueryResponse(contact.Id.ToString(),
            etag, contact.Description, contact.Email,
            new Name(contact.Name.FirstName, contact.Name.LastName),
            new Company(
                contact.Company.CompanyName,
                contact.Company.Street,
                contact.Company.HouseNumber,
                contact.Company.PostalCode,
                contact.Company.City,
                contact.Company.Country
            ));
    }
}