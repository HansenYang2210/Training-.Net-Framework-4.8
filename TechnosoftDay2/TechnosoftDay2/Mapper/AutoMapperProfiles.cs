using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechnosoftDay2.Models;

namespace TechnosoftDay2
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Country, CountryDto>();
            CreateMap<CountryDto, Country>();
        }
    }
}