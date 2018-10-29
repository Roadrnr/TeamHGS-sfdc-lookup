using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Salesforce.Common;

namespace TeamHGS_SFDCLookup.Pages
{
    public class CallbackModel : PageModel
    {
        private readonly IConfiguration _config;

        public CallbackModel(IConfiguration config)
        {
            _config = config;
        }

        public async void OnGet(string code)
        {
            var auth = new AuthenticationClient();
            await auth.WebServerAsync(
                _config["Salesforce:ConsumerKey"],
                _config["Salesforce:ConsumerSecret"], HttpUtility.UrlEncode("https://localhost:44346/"), code);
        }
    }
}