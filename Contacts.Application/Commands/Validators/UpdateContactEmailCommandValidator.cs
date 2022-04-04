using System;
using FluentValidation;

namespace Contacts.Application.Commands.Validators;

public class UpdateContactEmailCommandValidator : AbstractValidator<UpdateContactEmailCommand>
{
    public UpdateContactEmailCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotNull()
            .NotEqual(Guid.Empty);
        RuleFor(x => x.Etag)
            .MaximumLength(50);
        RuleFor(x => x.Email)
            .EmailAddress();
    }
}