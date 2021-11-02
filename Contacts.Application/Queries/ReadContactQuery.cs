using System;
using Contacts.Application.Queries.Responses;
using MediatR;

namespace Contacts.Application.Queries
{
    public class ReadContactQuery : IRequest<ReadContactQueryResponse>
    {
        public Guid Id { get; set; }
        public string Etag { get; set; }
    }
}