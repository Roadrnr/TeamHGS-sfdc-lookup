using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TeamHGS_SFDCLookup.Areas.Identity.Data;

namespace TeamHGS_SFDCLookup.Models
{
    public class TeamHGS_SFDCLookupContext : IdentityDbContext<LookupUser>
    {
        public TeamHGS_SFDCLookupContext(DbContextOptions<TeamHGS_SFDCLookupContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
