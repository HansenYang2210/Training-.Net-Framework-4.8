using MediatR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TechnosoftDay2.Context;
using FluentValidation;

namespace TechnosoftDay2.Request
{
    public static class Delete
    {
        public class Command : IRequest<DeleteResponse>
        {
            public Guid Id { get; set; }
            public byte[] VersionNumber { get; set; }
        }

        public class DeleteResponse
        {
            public Guid Id { get; set; }
            public byte[] VersionNumber { get; set; }
        }
        public class DeleteValidator : AbstractValidator<Command>
        {
            public DeleteValidator()
            {
                RuleFor(x => x.Id)
                    .NotEmpty().WithMessage("ID must be filled.");
            }
        }

        public class Handler : IRequestHandler<Command, DeleteResponse>
        {
            private readonly CountryContext _context;

            public Handler(CountryContext context)
            {
                _context = context;
            }

            public async Task<DeleteResponse> Handle(Command command, CancellationToken ct)
            {
                var country = await _context.Countries.FindAsync(command.Id);
                if (country == null)
                {
                    throw new KeyNotFoundException("Country not found");
                }

                _context.Countries.Remove(country);
                await _context.SaveChangesAsync(ct);

                return new DeleteResponse
                {
                    Id = country.Id,
                    VersionNumber = country.VersionNumber
                };
            }
        }
    }

}