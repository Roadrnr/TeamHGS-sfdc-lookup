using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Salesforce.Common;
using Salesforce.Force;
using System.Threading.Tasks;
using TeamHGS_SFDCLookup.Models;

namespace TeamHGS_SFDCLookup.Pages
{
    public class CallbackModel : PageModel
    {
        private readonly IConfiguration _config;
        public string InstanceUrl { get; set; }
        public string RefreshToken { get; set; }
        public string Token { get; set; }
        public string ApiVersion { get; set; }
        public List<Account> Accounts { get; set; }


        public CallbackModel(IConfiguration config)
        {
            _config = config;
        }

        public async Task<IActionResult> OnGetAsync(string code)
        {
            var auth = new AuthenticationClient();
            await auth.WebServerAsync(
                _config["Salesforce:ConsumerKey"],
                _config["Salesforce:ConsumerSecret"],
                "https://localhost:44346/callback",
                code,
                _config["Salesforce:TokenUrl"]);

            InstanceUrl = auth.InstanceUrl;
            RefreshToken = auth.RefreshToken;
            Token = auth.AccessToken;
            ApiVersion = auth.ApiVersion;

            var client = new ForceClient(InstanceUrl, Token, ApiVersion);

            var accounts = await client.QueryAsync<Account>("SELECT id, name, description FROM Account");
            Accounts = accounts.Records;

            return Page();
        }
    }
}