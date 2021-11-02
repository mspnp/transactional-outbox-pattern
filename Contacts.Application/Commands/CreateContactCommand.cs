using Contacts.Application.Commands.Responses;
using MediatR;

namespace Contacts.Application.Commands
{
    public class CreateContactCommand : IRequest<CreateContactCommandResponse>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public string CompanyName { get; set; }
    }
}