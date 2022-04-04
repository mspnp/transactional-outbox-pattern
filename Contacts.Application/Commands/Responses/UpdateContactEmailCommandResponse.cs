using System;

namespace Contacts.Application.Commands.Responses;

public record UpdateContactEmailCommandResponse(Guid Id, string Etag);