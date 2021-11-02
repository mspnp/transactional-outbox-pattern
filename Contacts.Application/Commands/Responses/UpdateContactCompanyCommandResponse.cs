using System;

namespace Contacts.Application.Commands.Responses
{
    public record UpdateContactCompanyCommandResponse(Guid Id, string Etag);
}