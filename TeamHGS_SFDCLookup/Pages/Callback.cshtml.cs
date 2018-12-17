using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Salesforce.Common;
using System.Threading.Tasks;

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

            if (User.Identity.IsAuthenticated)
            {
                if (TempData["InstanceUrl"] == null) TempData["InstanceUrl"] = auth.InstanceUrl;
                if (TempData["RefreshToken"] == null) TempData["RefreshToken"] = auth.RefreshToken;
                if (TempData["Token"] == null) TempData["Token"] = auth.AccessToken;
                if (TempData["ApiVersion"] == null) TempData["ApiVersion"] = auth.ApiVersion;
                return Page();
            }
            
            await auth.WebServerAsync(
                _config["Salesforce:ConsumerKey"],
                _config["Salesforce:ConsumerSecret"],
                _config["CallBackUrl"],
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