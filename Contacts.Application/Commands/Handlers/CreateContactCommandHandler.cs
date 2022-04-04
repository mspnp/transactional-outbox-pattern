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

public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, CreateContactCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateContactCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<CreateContactCommandResponse> Handle(CreateContactCommand request,
        CancellationToken cancellationToken)
    {
        var c = Contact.CreateNew();
        c.SetName(request.FirstName, request.LastName);
        c.SetCompany(request.CompanyName, "", "", "", "", "");
        c.SetDescription(request.Description);
        c.SetEmail(request.Email);
        _unitOfWork.ContactsRepo.Create(c);

        var result = await _unitOfWork.CommitAsync(cancellationToken);
        var cResult = result.FirstOrDefault(r => r is DataObject<Contact>);
        if (cResult != null)
        {
            return new CreateContactCommandResponse(Guid.Parse(cResult.Id), cResult.Etag);
        }

        throw new Exception("Error saving contact");
    }
}