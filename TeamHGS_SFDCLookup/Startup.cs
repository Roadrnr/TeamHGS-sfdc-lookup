using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TeamHGS_SFDCLookup.Services;
using ForceClient = Salesforce.Force.ForceClient;
using IForceClient = Salesforce.Force.IForceClient;
using TeamHGS_SFDCLookup.Models;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using TeamHGS_SFDCLookup.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace TeamHGS_SFDCLookup
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddAuthentication()
                .AddSalesforce(options =>
                {
                    options.AuthorizationEndpoint = Configuration["Salesforce:AuthUrl"];
                    options.ClientId = Configuration["Salesforce:ConsumerKey"];
                    options.ClientSecret = Configuration["Salesforce:ConsumerSecret"];
                    options.CallbackPath = "/signin-salesforce";
                    options.SaveTokens = true;
                    options.TokenEndpoint = Configuration["Salesforce:TokenUrl"];
                });

            // Add framework services.
            services.AddDbContext<TeamHGS_SFDCLookupContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("TeamHGS_SFDCLookupContextConnection")));

            services.AddTransient<IForceClient, ForceClient>();
            services.AddTransient<ILookup, Lookup>();
            services.AddTransient<IImportService, ImportService>();
            services.AddTransient<IExportService, ExportService>();
            services.AddTransient<IAzureStorageService, AzureStorageService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("TeamHGS_SFDCLookupContextConnection")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseHangfireServer();
            app.UseHangfireDashboard();
            app.UseMvc();
        }
    }
}
