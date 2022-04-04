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

public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand, DeleteContactCommandResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContactCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DeleteContactCommandResponse> Handle(DeleteContactCommand request,
        CancellationToken cancellationToken)
    {
        await _unitOfWork.ContactsRepo.DeleteAsync(request.Id, request.Etag);

        var result = await _unitOfWork.CommitAsync(cancellationToken);
        var cResult = result.FirstOrDefault(r => r is DataObject<Contact>);
        if (cResult != null)
        {
            return new DeleteContactCommandResponse(Guid.Parse(cResult.Id), cResult.Etag);
        }

        throw new Exception("Error saving contact");
    }
}