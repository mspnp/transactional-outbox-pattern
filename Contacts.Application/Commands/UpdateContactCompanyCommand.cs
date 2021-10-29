using System;
using Contacts.Application.Commands.Responses;
using MediatR;

namespace Contacts.Application.Commands
{
    public class UpdateContactCompanyCommand : IRequest<UpdateContactCompanyCommandResponse>
    {
        public Guid Id { get; set; }
        public string Etag { get; set; }
        public string CompanyName { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}