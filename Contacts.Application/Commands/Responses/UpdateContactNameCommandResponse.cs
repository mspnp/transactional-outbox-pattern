using System;

namespace Contacts.Application.Commands.Responses;

public record UpdateContactNameCommandResponse(Guid Id, string Etag);