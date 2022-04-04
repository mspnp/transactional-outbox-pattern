using FluentValidation;

namespace Contacts.Application.Commands.Validators;

public class CreateContactCommandValidator : AbstractValidator<CreateContactCommand>
{
    public CreateContactCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(x => x.Email)
            .EmailAddress();
        RuleFor(x => x.Description)
            .MaximumLength(500);
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .MaximumLength(250);
    }
}