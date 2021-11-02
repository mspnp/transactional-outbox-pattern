namespace Contacts.Application.Models
{
    public class ReadContactDto
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public NameDto Name { get; set; }
        public CompanyDto Company { get; set; }
    }

    public class NameDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class CompanyDto
    {
        public string CompanyName { get; set; }
        public string Street { get; set; }
        public string HouseNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }
}