using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Web;
using TechnosoftDay2.Context;

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