using System;

namespace Contacts.Application.Commands.Responses;

public record CreateContactCommandResponse(Guid Id, string Etag);