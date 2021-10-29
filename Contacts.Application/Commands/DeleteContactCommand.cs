using System;
using Contacts.Application.Commands.Responses;
using MediatR;

namespace Contacts.Application.Commands
{
    public class DeleteContactCommand : IRequest<DeleteContactCommandResponse>
    {
        public Guid Id { get; set; }
        public string Etag { get; set; }
    }
}