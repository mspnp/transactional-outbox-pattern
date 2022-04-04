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

public class UpdateContactDescriptionCommandHandler : IRequestHandler<UpdateContactDescriptionCommand,
    UpdateContactDescriptionCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContactDescriptionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateContactDescriptionCommandResponse> Handle(UpdateContactDescriptionCommand request,
        CancellationToken cancellationToken)
    {
        // Read without etag --> use current version of contact
        var (contact, etag) = await _unitOfWork.ContactsRepo.ReadAsync(request.Id, null);
        contact.SetDescription(request.Description);

        // If etag was provided by client, use it to make sure it's definitely the intended version.
        // Otherwise, use etag from previous read.
        _unitOfWork.ContactsRepo.Update(contact, string.IsNullOrWhiteSpace(request.Etag) ? etag : request.Etag);

        var result = await _unitOfWork.CommitAsync(cancellationToken);
        var cResult = result.FirstOrDefault(r => r is DataObject<Contact>);
        if (cResult != null)
        {
            return new UpdateContactDescriptionCommandResponse(Guid.Parse(cResult.Id), cResult.Etag);
        }

        throw new Exception("Error saving contact");
    }
}