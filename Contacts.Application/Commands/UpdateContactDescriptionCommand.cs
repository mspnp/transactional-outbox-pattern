using System;
using Contacts.Application.Commands.Responses;
using MediatR;

namespace Contacts.Application.Commands;

public class UpdateContactDescriptionCommand : IRequest<UpdateContactDescriptionCommandResponse>
{
    public Guid Id { get; set; }
    public string Etag { get; set; }
    public string Description { get; set; }
}