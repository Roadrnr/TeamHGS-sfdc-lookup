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
        public bool IsAuthenticated { get; set; }
        public bool ByName { get; set; }
        public bool ByEmail { get; set; }
        public bool ByCompany { get; set; }
        public string ReturnUrl { get; set; }

        public string InstanceUrl { get; set; }
        public string RefreshToken { get; set; }
        public string Token { get; set; }
        public string ApiVersion { get; set; }
        public List<Account> Accounts { get; set; }

        public IndexModel(IConfiguration config)
        {
            _config = config;
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
                InstanceUrl = TempData.Peek("InstanceUrl").ToString();
                RefreshToken = TempData.Peek("RefreshToken").ToString();
                Token = TempData.Peek("Token").ToString();
                ApiVersion = TempData.Peek("ApiVersion").ToString();

                var client = new ForceClient(TempData.Peek("InstanceUrl").ToString(), TempData.Peek("Token").ToString(),
                    TempData.Peek("ApiVersion").ToString());
                var query = "SELECT id, name FROM Contact";

                if (queryParams.ByName) query = $"{query} WHERE name LIKE '%{queryParams.Name}%'";
                if (queryParams.ByEmail) query = $"{query} WHERE email LIKE '%{queryParams.Email}%'";
                //if (queryParams.ByCompany) query = $"{query} WHERE accountid = {queryParams.Company}";
                query = $"{query} ORDER BY name";

                var accounts = await client.QueryAsync<Account>(query);
                Accounts = accounts.Records;
            }

            return Page();
        }
    }
}
