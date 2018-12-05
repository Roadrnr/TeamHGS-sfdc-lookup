using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Salesforce.Common;
using Salesforce.Common.Models.Json;
using TeamHGS_SFDCLookup.Models;
using TeamHGS_SFDCLookup.Services;

namespace TeamHGS_SFDCLookup.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly ILookup _lookup;
        private readonly IImportService _importService;
        private readonly IExportService _exportService;

        public bool IsAuthenticated { get; set; }
        public string ReturnUrl { get; set; }

        [BindProperty]
        public QueryParameters QueryParams { get; set; }
        
        public List<Person> Accounts { get; set; }
        public SalesForceCredential SalesForceCredential { get; set; }

        public IndexModel(IConfiguration config, ILookup lookup, IImportService importService, IExportService exportService)
        {
            _config = config;
            _lookup = lookup;
            _importService = importService;
            _exportService = exportService;
            Accounts = new List<Person>();
        }

        public void OnGet()
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
                SalesForceCredential = new SalesForceCredential
                {
                    InstanceUrl = TempData.Peek("InstanceUrl") as string,
                    RefreshToken = TempData.Peek("RefreshToken").ToString(),
                    Token = TempData.Peek("Token").ToString(),
                    ApiVersion = TempData.Peek("ApiVersion").ToString()
                };
            }
        }

        public async Task<IActionResult> OnPostImportAsync()
        {
            IsAuthenticated = true;
            SalesForceCredential = new SalesForceCredential
            {
                InstanceUrl = TempData.Peek("InstanceUrl") as string,
                RefreshToken = TempData.Peek("RefreshToken").ToString(),
                Token = TempData.Peek("Token").ToString(),
                ApiVersion = TempData.Peek("ApiVersion").ToString()
            };

            if (!QueryParams.Company && !QueryParams.Email && !QueryParams.Name)
            {
                ModelState.AddModelError("QueryParams.Name","You must choose at least one search criteria.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (QueryParams.ImportFile.Length > 0)
            {
                Accounts = await _importService.Import(QueryParams, SalesForceCredential);
            }
            else
            {
                var lookupPerson = new Person();
                Accounts = await _lookup.LookupContact(QueryParams, lookupPerson, SalesForceCredential);
            }
            return _exportService.ExportResults(Accounts);
            //return Page();
        }

        public async Task<IActionResult> OnPostExportAsync()
        {
            //await _exportService.ExportResults();
            return Page();
        }
    }
}
