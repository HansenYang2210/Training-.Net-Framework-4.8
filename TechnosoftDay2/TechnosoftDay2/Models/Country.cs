using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechnosoftDay2.Models
{
    public class Country
    {
        public Guid Id { get; set; }
        public byte[] VersionNumber { get; set; }
        public string Name { get; set; }
        public string CallingCode { get; set; }

    }
}