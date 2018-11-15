using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Salesforce.Common;
using Salesforce.Common.Models.Json;
using Salesforce.Force;
using TeamHGS_SFDCLookup.Models;
using TeamHGS_SFDCLookup.Services;

namespace TeamHGS_SFDCLookup.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly ILookup _lookup;
        public int ObuId { get; set; }
        public bool IsAuthenticated { get; set; }
        public string ReturnUrl { get; set; }

        
        public List<Person> Accounts { get; set; }
        public SalesForceCredential SalesForceCredential { get; set; }

        public IndexModel(IConfiguration config, ILookup lookup)
        {
            _config = config;
            _lookup = lookup;
            SalesForceCredential = new SalesForceCredential
            {
                InstanceUrl = TempData.Peek("InstanceUrl").ToString(),
                RefreshToken = TempData.Peek("RefreshToken").ToString(),
                Token = TempData.Peek("Token").ToString(),
                ApiVersion = TempData.Peek("ApiVersion").ToString()
            };
        }

        public async Task<IActionResult> OnGetAsync(QueryParameters queryParams)
        {
            if (TempData.Peek("Token") == null)
            {
                IsAuthenticated = false;
                ReturnUrl =
                    Common.FormatAuthUrl(
                        _config["Salesforce:AuthUrl"], // if using sandbox org then replace login with test
                        ResponseTypes.Code,
                        _config["Salesforce:ConsumerKey"],
                        HttpUtility.UrlEncode("https://localhost:44346/callback"),
                        DisplayTypes.Popup);
            }
            else
            {
                IsAuthenticated = true;
                Accounts = await _lookup.LookupContact(queryParams, SalesForceCredential);
            }

            return Page();
        }
    }
}
