using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TeamHGS_SFDCLookup.Pages
{
    public class ClearTokensModel : PageModel
    {
        public IActionResult OnGet()
        {
            TempData.Clear();
            return RedirectToPage("Index");
        }
    }
}