using AutoMapper;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Data.Entity;
using TechnosoftDay2.Context;
using TechnosoftDay2.Models;
using TechnosoftDay2.Response;

namespace TechnosoftDay2.Request
{
    public static class Retrieve
    {
        public class ListQuery : IRequest<ListResponse>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public string Select { get; set; } = "";
        }
        public class ListQueryHandler : IRequestHandler<ListQuery, ListResponse>
        {
            private readonly CountryContext _context;
            private readonly IMapper _mapper;

            public ListQueryHandler(CountryContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<ListResponse> Handle(ListQuery query, CancellationToken ct)
            {
                //var fields = query.Select.Any() ? query.Select : new List<string> { "id", "name", "versionNumber", "callingCode" };
                var countriesQuery = _context.Countries
                    .OrderByDescending(x => x.Id)
                    .AsQueryable();


                var totalRecords = await _context.Countries.CountAsync(ct);

                if (query.PageNumber == 0 && query.PageSize == 0)
                {
                    query.PageNumber = 1;
                    query.PageSize = totalRecords;
                }

                var pagedCountries = await countriesQuery.
                    Skip((query.PageNumber - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync(ct);

                var responseData = pagedCountries.Select(c => new CountryDto
                {
                    Id = query.Select == null || !query.Select.Any() || query.Select.Contains("id") ? c.Id : default,
                    VersionNumber = query.Select == null || !query.Select.Any() || query.Select.Contains("versionNumber") ? c.VersionNumber : null,
                    Name = query.Select == null || !query.Select.Any() || query.Select.Contains("name") ? c.Name : null,
                    CallingCode = query.Select == null || !query.Select.Any() || query.Select.Contains("callingCode") ? c.CallingCode : null
                }).ToList();


                var hasNextPage = (query.PageNumber * query.PageSize) < totalRecords;

                return new ListResponse
                {
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize,
                    TotalRecords = totalRecords,
                    Data = responseData,
                    Next = hasNextPage
                };
            }
        }
        public class Query : IRequest<CountryDto>
        {
            public Guid Id { get; set; }

            public string Select { get; set; } = "";

        }
        public class QueryHandler : IRequestHandler<Query, CountryDto>
        {
            private readonly CountryContext _context;
            private readonly IMapper _mapper;

            public QueryHandler(CountryContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<CountryDto> Handle(Query query, CancellationToken cancellationToken)
            {
                var country = await _context.Countries
                    .Where(c => c.Id == query.Id)
                    .SingleOrDefaultAsync(cancellationToken);

                if (country == null)
                {
                    return null;
                }

                var countryDto = _mapper.Map<CountryDto>(country);

                if (query.Select == "")
                {
                    return countryDto;
                }
                else
                {
                    if (!query.Select.Contains("id"))
                        countryDto.Id = default;
                    if (!query.Select.Contains("versionNumber"))
                        countryDto.VersionNumber = null;
                    if (!query.Select.Contains("name"))
                        countryDto.Name = null;
                    if (!query.Select.Contains("callingCode"))
                        countryDto.CallingCode = null;

                    return countryDto;
                }

                //return _mapper.Map<CountryDto>(country);
            }
        }




    }

}