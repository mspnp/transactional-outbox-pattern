using System.Collections.Generic;

namespace Contacts.Application.Models
{
    public class ContactsListItemDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string CompanyName { get; set; }
    }

    public class ReadAllContactsDto
    {
        public bool HasMore { get; set; }
        public List<ContactsListItemDto> Items { get; set; }
    }
}