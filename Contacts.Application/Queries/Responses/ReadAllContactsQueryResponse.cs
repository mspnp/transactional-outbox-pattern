using System.Collections.Generic;

namespace Contacts.Application.Queries.Responses;

public record ContactsListModel(
    string Id,
    string Etag,
    string Email,
    string FullName,
    string CompanyName
);

public record ReadAllContactsQueryResponse(
    bool HasMore, string ContinuationToken, List<ContactsListModel> Items
);