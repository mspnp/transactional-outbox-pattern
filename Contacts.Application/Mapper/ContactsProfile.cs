using AutoMapper;
using Contacts.Application.Commands;
using Contacts.Application.Models;
using Contacts.Application.Queries.Responses;

namespace Contacts.Application.Mapper;

public class ContactsProfile : Profile
{
    public ContactsProfile()
    {
        CreateMap<CreateContactDto, CreateContactCommand>();
        CreateMap<UpdateContactCompanyDto, UpdateContactCompanyCommand>();
        CreateMap<UpdateContactDescriptionDto, UpdateContactDescriptionCommand>();
        CreateMap<UpdateContactEmailDto, UpdateContactEmailCommand>();
        CreateMap<UpdateContactNameDto, UpdateContactNameCommand>();
        CreateMap<Name, NameDto>();
        CreateMap<Company, CompanyDto>();
        CreateMap<ReadContactQueryResponse, ReadContactDto>();
        CreateMap<ContactsListModel, ContactsListItemDto>();
        CreateMap<ReadAllContactsQueryResponse, ReadAllContactsDto>();
    }
}