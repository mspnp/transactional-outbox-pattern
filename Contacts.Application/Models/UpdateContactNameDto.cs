namespace Contacts.Application.Models;

public record UpdateContactNameDto(
    string FirstName,
    string LastName
);