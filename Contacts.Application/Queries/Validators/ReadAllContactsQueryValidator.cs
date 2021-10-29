using FluentValidation;

namespace Contacts.Application.Queries.Validators
{
    public class ReadAllContactsQueryValidator : AbstractValidator<ReadAllContactsQuery>
    {
        public ReadAllContactsQueryValidator()
        {
            RuleFor(x => x.PageSize)
                .GreaterThan(0);
        }
    }
}