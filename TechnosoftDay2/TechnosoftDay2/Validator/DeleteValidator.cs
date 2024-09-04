using FluentValidation;
using static TechnosoftDay2.Request.Delete;

namespace TechnosoftDay2.Validator
{
    public class DeleteValidator : AbstractValidator<Command>
    {
        public DeleteValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("ID must be filled.");
        }
    }
}
