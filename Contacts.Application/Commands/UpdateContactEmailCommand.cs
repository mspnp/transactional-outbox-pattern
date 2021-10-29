using System;
using Contacts.Application.Commands.Responses;
using MediatR;

namespace Contacts.Application.Commands
{
    public class UpdateContactEmailCommand : IRequest<UpdateContactEmailCommandResponse>
    {
        public Guid Id { get; set; }
        public string Etag { get; set; }
        public string Email { get; set; }
    }
}