using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using TechnosoftDay2.Context;
using System.Data.Entity;

namespace TechnosoftDay2.Request
{
    public static class Create
    {
        public class Command : IRequest<CreateResponse>
        {
            public string Name { get; set; }
            public string CallingCode { get; set; }
        }

        public class CreateResponse
        {
            public Guid Id { get; set; }
            public byte[] VersionNumber { get; set; }
        }

        public class Handler : IRequestHandler<Command, CreateResponse>
        {
            private readonly CountryContext _context;
            private readonly IMapper _mapper;

            public Handler(CountryContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<CreateResponse> Handle(Command command, CancellationToken ct)
            {
                var country = new Models.Country
                {
                    Id = Guid.NewGuid(),
                    Name = command.Name,
                    CallingCode = command.CallingCode,
                    VersionNumber = GenerateRandomByteArray(18)
                };
                string newVersionNumber = country.VersionNumber.ToString();

                var checkCountry = await _context.Countries
                .FirstOrDefaultAsync(c => c.Name == command.Name, ct);

                if (checkCountry == null)
                {
                    _context.Countries.Add(country);
                    await _context.SaveChangesAsync(ct);
                }
                else
                {
                    throw new KeyNotFoundException("Country already exists");
                }

                return new CreateResponse
                {
                    Id = country.Id,
                    VersionNumber = country.VersionNumber
                };
            }

            public byte[] GenerateRandomByteArray(int length)
            {
                byte[] randomBytes = new byte[length];
                using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                {
                    rng.GetBytes(randomBytes);
                }
                return randomBytes;
            }
        }
    }

}