namespace Contacts.Application.Models;

public record UpdateContactCompanyDto(
    string CompanyName,
    string Street,
    string HouseNumber,
    string PostalCode,
    string City,
    string Country
);