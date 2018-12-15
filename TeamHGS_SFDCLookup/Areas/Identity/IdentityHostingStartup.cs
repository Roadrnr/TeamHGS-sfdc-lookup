using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeamHGS_SFDCLookup.Areas.Identity.Data;
using TeamHGS_SFDCLookup.Models;

[assembly: HostingStartup(typeof(TeamHGS_SFDCLookup.Areas.Identity.IdentityHostingStartup))]
namespace TeamHGS_SFDCLookup.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<TeamHGS_SFDCLookupContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("TeamHGS_SFDCLookupContextConnection")));

                services.AddDefaultIdentity<LookupUser>()
                    .AddEntityFrameworkStores<TeamHGS_SFDCLookupContext>();
            });
        }
    }
}