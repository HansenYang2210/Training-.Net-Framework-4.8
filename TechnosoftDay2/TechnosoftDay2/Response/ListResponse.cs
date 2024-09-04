using TechnosoftDay2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechnosoftDay2.Response
{


    public class ListResponse
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public List<CountryDto> Data { get; set; }
        public bool Next { get; set; }
    }
}