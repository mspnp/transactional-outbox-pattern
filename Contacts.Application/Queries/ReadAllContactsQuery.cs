using Contacts.Application.Queries.Responses;
using MediatR;

namespace Contacts.Application.Queries;

public class ReadAllContactsQuery : IRequest<ReadAllContactsQueryResponse>
{
    public int PageSize { get; set; }
    public string ContinuationToken { get; set; }
}