using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

public class ValidatorBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly AbstractValidator<TRequest> _validator;

    public ValidatorBehaviour(AbstractValidator<TRequest> validator)
    {
        _validator = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (next is null)
        {
            throw new ArgumentNullException(nameof(next));
        }

        // Validate the request
        await _validator.ValidateAndThrowAsync(request, cancellationToken);

        // Call the next delegate in the pipeline
        return await next();
    }
}
