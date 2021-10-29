namespace Contacts.Application.Queries.Responses
{
    public record ReadContactQueryResponse(
        string Id,
        string Etag,
        string Description,
        string Email,
        Name Name,
        Company Company
    );

    public record Name(string FirstName, string LastName);

    public record Company(string CompanyName, string Street, string HouseNumber, string PostalCode, string City,
        string Country);
}