using System;

namespace Contacts.Application.Commands.Responses
{
    public record UpdateContactDescriptionCommandResponse(Guid Id, string Etag);
}