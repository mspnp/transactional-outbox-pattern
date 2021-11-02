using System;
using FluentValidation;

namespace Contacts.Application.Commands.Validators
{
    public class UpdateContactDescriptionCommandValidator : AbstractValidator<UpdateContactDescriptionCommand>
    {
        public UpdateContactDescriptionCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .NotEqual(Guid.Empty);
            RuleFor(x => x.Etag)
                .MaximumLength(50);
            RuleFor(x => x.Description)
                .MaximumLength(500);
        }
    }
}