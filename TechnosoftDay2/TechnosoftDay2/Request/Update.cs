using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using TechnosoftDay2.Context;
using FluentValidation;

namespace TechnosoftDay2.Request
{
    public static class Update
    {
        public class Command : IRequest<UpdateResponse>
        {
            public Guid Id { get; set; }
            public byte[] VersionNumber { get; set; }
            public List<Operation> Operations { get; set; }
        }

        public class UpdateResponse
        {
            public Guid Id { get; set; }
            public byte[] VersionNumber { get; set; }
        }

        public class Operation
        {
            public string Op { get; set; }
            public string Path { get; set; }
            public string Value { get; set; }
        }

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
                    .NotEmpty().WithMessage("Path is required.")
                    .Matches(@"^[^\d]*$").WithMessage("Path cannot contain numbers.");

                RuleFor(x => x.Value)
                    .NotEmpty().WithMessage("Value is required.")
                    .Matches(@"^[^\d]*$").WithMessage("Value cannot contain numbers.");
            }
        }

        public class Handler : IRequestHandler<Command, UpdateResponse>
        {
            private readonly CountryContext _context;

            public Handler(CountryContext context)
            {
                _context = context;
            }

            public async Task<UpdateResponse> Handle(Command command, CancellationToken ct)
            {
                var country = await _context.Countries.FindAsync(command.Id);
                if (country == null)
                {
                    throw new KeyNotFoundException("Country not found");
                }

                foreach (var op in command.Operations)
                {
                    if (op.Path == "name")
                    {
                        country.Name = op.Value;
                    }

                    if (op.Path == "callingCode")
                    {
                        country.CallingCode = op.Value;
                    }

                }

                //VersioNumber
                await _context.SaveChangesAsync(ct);

                return new UpdateResponse
                {
                    Id = country.Id,
                    VersionNumber = country.VersionNumber
                };
            }
        }
    }

}