
using System.Data.Entity;
using TechnosoftDay2.Models;

namespace TechnosoftDay2.Context
{
    public class CountryContext : DbContext
    {
            public DbSet<Country> Countries { get; set; }

            public CountryContext() : base("name=CountryDatabase") { }
    }
}