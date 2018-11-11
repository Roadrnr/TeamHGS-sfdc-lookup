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

            if (TempData["InstanceUrl"] == null) TempData["InstanceUrl"] = auth.InstanceUrl;
            if (TempData["RefreshToken"] == null) TempData["RefreshToken"] = auth.RefreshToken;
            if (TempData["Token"] == null) TempData["Token"] = auth.AccessToken;
            if (TempData["ApiVersion"] == null) TempData["ApiVersion"] = auth.ApiVersion;

            if (auth.AccessToken != null) return RedirectToPage("Index");

            return Page();
        }
    }
}