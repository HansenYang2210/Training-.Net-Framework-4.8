using FluentValidation;
using static TechnosoftDay2.Request.Update;

namespace TechnosoftDay2.Validator
{
    public class UpdateValidator : AbstractValidator<Command>
    {
        public UpdateValidator()
        {
            RuleFor(x => x.VersionNumber)
                .NotEmpty().WithMessage("VersionNumber is required.");

            RuleFor(x => x.Operations)
                .NotNull().WithMessage("Operations is required.")
                .Must(operations => operations != null && operations.Count > 0).WithMessage("At least one operation is required.")
                .ForEach(operation =>
                    operation.SetValidator(new OperationValidator())
                );
        }
    }

    public class OperationValidator : AbstractValidator<Operation>
    {
        public OperationValidator()
        {
            RuleFor(x => x.Op)
                .NotEmpty().WithMessage("Op is required.")
                .Must(op => op == "replace" || op == "add" || op == "remove").WithMessage("Op must be 'replace'.");

            RuleFor(x => x.Path)
                .NotEmpty().WithMessage("Path is required.");

            RuleFor(x => x.Value)
                .NotEmpty().WithMessage("Value is required.");
        }
    }
}