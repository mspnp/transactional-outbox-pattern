using System;
using FluentValidation;

namespace Contacts.Application.Commands.Validators;

public class UpdateContactCompanyCommandValidator : AbstractValidator<UpdateContactCompanyCommand>
{
    public UpdateContactCompanyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .NotEqual(Guid.Empty);
        RuleFor(x => x.Etag)
            .MaximumLength(50);
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .MaximumLength(250);
        RuleFor(x => x.Street)
            .MaximumLength(250);
        RuleFor(x => x.HouseNumber)
            .MaximumLength(10);
        RuleFor(x => x.PostalCode)
            .MaximumLength(25);
        RuleFor(x => x.City)
            .MaximumLength(250);
        RuleFor(x => x.Country)
            .MaximumLength(250);
    }
}