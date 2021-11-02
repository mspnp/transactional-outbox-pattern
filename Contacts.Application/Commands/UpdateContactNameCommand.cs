using System;
using Contacts.Application.Commands.Responses;
using MediatR;

namespace Contacts.Application.Commands
{
    public class UpdateContactNameCommand : IRequest<UpdateContactNameCommandResponse>
    {
        public Guid Id { get; set; }
        public string Etag { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}