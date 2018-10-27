using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Salesforce.Common;
using Salesforce.Common.Models.Json;
using Salesforce.Force;
using TeamHGS_SFDCLookup.Models;

namespace TeamHGS_SFDCLookup.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;
        public int OBU { get; set; }
        public Enums.OriginatingBusinessUnits OBUs { get; set; }

        public bool ByName { get; set; }
        public bool ByEmail { get; set; }
        public bool ByCompany { get; set; }
        public string ReturnUrl { get; set; }

        public IndexModel(IConfiguration config)
        {
            _config = config;
        }

        public void OnGet()
        {
            //var auth = new AuthenticationClient();

            //await auth.UsernamePasswordAsync(
            //    _config["Salesforce:ConsumerKey"],
            //    _config["Salesforce:ConsumerSecret"],
            //    _config["Salesforce:Username"],
            //    _config["Salesforce:Password"]);

            ReturnUrl =
                Common.FormatAuthUrl(
                    _config["Salesforce:AuthUrl"], // if using sandbox org then replace login with test
                    ResponseTypes.Code,
                    _config["Salesforce:ConsumerKey"],
                    HttpUtility.UrlEncode("https://localhost:44346/"));
        }
    }
}
