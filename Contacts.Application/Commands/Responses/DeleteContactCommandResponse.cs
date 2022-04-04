using System;

namespace Contacts.Application.Commands.Responses;

public record DeleteContactCommandResponse(Guid Id, string Etag);