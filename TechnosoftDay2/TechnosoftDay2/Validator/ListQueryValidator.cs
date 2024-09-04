using FluentValidation;
using static TechnosoftDay2.Request.Retrieve;

public class ListQueryValidator : AbstractValidator<ListQuery>
{
    public ListQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0).WithMessage("Page number must be greater than zero.");

        RuleFor(x => x.PageSize)
            .GreaterThan(0).WithMessage("Page size must be greater than zero.")
            .LessThanOrEqualTo(100).WithMessage("Page size must be 100 or less.");
    }
}
