using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contacts.Application.Commands.Responses;
using Contacts.Domain;
using Contacts.Infrastructure;
using Contacts.Infrastructure.Context;
using MediatR;

namespace Contacts.Application.Commands.Handlers;

public class UpdateContactNameCommandHandler : IRequestHandler<UpdateContactNameCommand,
    UpdateContactNameCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContactNameCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateContactNameCommandResponse> Handle(UpdateContactNameCommand request,
        CancellationToken cancellationToken)
    {
        // Read without etag --> use current version of contact
        var (contact, etag) = await _unitOfWork.ContactsRepo.ReadAsync(request.Id, null);
        contact.SetName(request.FirstName, request.LastName);

        // If etag was provided by client, use it to make sure it's definitely the intended version.
        // Otherwise, use etag from previous read.
        _unitOfWork.ContactsRepo.Update(contact, string.IsNullOrWhiteSpace(request.Etag) ? etag : request.Etag);

        var result = await _unitOfWork.CommitAsync(cancellationToken);
        var cResult = result.FirstOrDefault(r => r is DataObject<Contact>);
        if (cResult != null)
        {
            return new UpdateContactNameCommandResponse(Guid.Parse(cResult.Id), cResult.Etag);
        }

        throw new Exception("Error saving contact");
    }
}