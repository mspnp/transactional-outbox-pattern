using System;
using FluentValidation;

namespace Contacts.Application.Queries.Validators
{
    public class ReadContactQueryValidator : AbstractValidator<ReadContactQuery>
    {
        public ReadContactQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotNull()
                .NotEqual(Guid.Empty);
            RuleFor(x => x.Etag)
                .MaximumLength(50);
        }
    }
}