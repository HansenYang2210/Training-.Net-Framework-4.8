using FluentValidation;
using static TechnosoftDay2.Request.Retrieve;

namespace TechnosoftDay2.Validator
{
    public class GetQueryValidator : AbstractValidator<Query>
    {
        public GetQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID must be filled.");
        }
    }
}
