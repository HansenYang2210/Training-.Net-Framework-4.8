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
using FluentValidation;
using System.Collections.Generic;

namespace TechnosoftDay2.Request
{
    public static class Retrieve
    {
        public class ListQuery : IRequest<ListResponse>
        {
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public Guid ID { get; set; }

            public int GetPageNumberOrDefault() => PageNumber ?? 1;
            public int GetPageSizeOrDefault() => PageSize ?? 10;
            public string Select { get; set; } = "";
        }
        public class ListResponse
        {
            public int? PageNumber { get; set; }
            public int? PageSize { get; set; }
            public int TotalRecords { get; set; }
            public List<CountryDto> Data { get; set; }
            public bool Next { get; set; }
        }
        public class ListQueryValidator : AbstractValidator<ListQuery>
        {
            public ListQueryValidator()
            {
                RuleFor(x => x.PageNumber)
            .GreaterThan(0).When(x => x.PageNumber.HasValue)
            .WithMessage("Page number must be greater than zero.");

                RuleFor(x => x.PageSize)
                    .GreaterThan(0).When(x => x.PageSize.HasValue)
                    .WithMessage("Page size must be greater than zero.")
                    .LessThanOrEqualTo(100).When(x => x.PageSize.HasValue)
                    .WithMessage("Page size must be 100 or less.");
            }
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
                var pageNumber = query.GetPageNumberOrDefault();
                var pageSize = query.GetPageSizeOrDefault();

                var countriesQuery = _context.Countries
                    .OrderByDescending(x => x.Id)
                    .AsQueryable();

                if (query.ID.ToString() != "00000000-0000-0000-0000-000000000000")
                {
                    countriesQuery = countriesQuery.Where(c => c.Id.ToString() == query.ID.ToString());
                }

                var totalRecords = await _context.Countries.CountAsync(ct);


                if (query.PageSize == null)
                {
                    pageSize = totalRecords;
                }

                var pagedCountries = await countriesQuery
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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
                    PageNumber = pageNumber,
                    PageSize = pageSize,
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
        public class GetQueryValidator : AbstractValidator<Query>
        {
            public GetQueryValidator()
            {
                RuleFor(x => x.Id)
                    .NotEmpty().WithMessage("ID must be filled.");
            }
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