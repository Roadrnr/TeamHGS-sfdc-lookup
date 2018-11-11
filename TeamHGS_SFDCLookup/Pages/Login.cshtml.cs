using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TeamHGS_SFDCLookup.Pages
{
    public class LoginModel : PageModel
    {
        public IActionResult OnGet(string returnUrl = "/callback")
        {
            return Challenge(new AuthenticationProperties() { RedirectUri = returnUrl });
        }
    }
}