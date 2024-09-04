using FluentValidation;
using static TechnosoftDay2.Request.Create;

namespace TechnosoftDay2.Validator
{
    public class CreateValidator : AbstractValidator<Command>
    {
        public CreateValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Matches(@"^[^\d]*$").WithMessage("Name cannot contain numbers.");

            RuleFor(x => x.CallingCode)
            .NotEmpty().WithMessage("CallingCode is required.")
            .Matches(@"^[^\d]*$").WithMessage("CallingCode cannot contain numbers.");
        }
    }
}