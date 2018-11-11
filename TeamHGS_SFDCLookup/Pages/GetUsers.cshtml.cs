using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Salesforce.Common.Models.Json;
using Salesforce.Force;
using TeamHGS_SFDCLookup.Models;

namespace TeamHGS_SFDCLookup.Pages
{
    public class GetUsersModel : PageModel
    {
        public List<Account> Users { get; set; }
        public string InstanceUrl { get; set; }
        public string RefreshToken { get; set; }
        public string Token { get; set; }
        public string ApiVersion { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            InstanceUrl = TempData.Peek("InstanceUrl").ToString();
            RefreshToken = TempData.Peek("RefreshToken").ToString();
            Token = TempData.Peek("Token").ToString();
            ApiVersion = TempData.Peek("ApiVersion").ToString();


            var client = new ForceClient(InstanceUrl, Token, ApiVersion);

            var accounts = await client.QueryAsync<Account>("SELECT id, name FROM User ORDER BY name");
            Users = accounts.Records;

            return Page();

        }
    }
}