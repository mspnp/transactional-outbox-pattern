namespace Contacts.Application.Models;

public record CreateContactDto(
    string FirstName,
    string LastName,
    string Email,
    string Description,
    string CompanyName
);