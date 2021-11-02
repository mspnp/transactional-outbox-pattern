using System;
using FluentValidation;

namespace Contacts.Application.Commands.Validators
{
    public class UpdateContactNameCommandValidator : AbstractValidator<UpdateContactNameCommand>
    {
        public UpdateContactNameCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .NotEqual(Guid.Empty);
            RuleFor(x => x.Etag)
                .MaximumLength(50);
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(100);
        }
    }
}