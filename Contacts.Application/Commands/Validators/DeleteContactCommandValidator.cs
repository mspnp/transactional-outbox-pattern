using System;
using FluentValidation;

namespace Contacts.Application.Commands.Validators
{
    public class DeleteContactCommandValidator : AbstractValidator<DeleteContactCommand>
    {
        public DeleteContactCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .NotEqual(Guid.Empty);
            RuleFor(x => x.Etag)
                .MaximumLength(50);
        }
    }
}