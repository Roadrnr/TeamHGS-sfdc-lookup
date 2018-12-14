using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<IndexModel> _logger;
        private static HttpClient httpClient = new HttpClient();
        public bool IsAuthenticated { get; set; }
        public int ReturnInt { get; set; }
        public string ReturnUrl { get; set; }

        [BindProperty]
        public QueryParameters QueryParams { get; set; }
        
        public List<Person> Accounts { get; set; }
        public SalesForceCredential SalesForceCredential { get; set; }

        public IndexModel(IConfiguration config,
            ILookup lookup,
            IImportService importService,
            IExportService exportService,
            ILogger<IndexModel> logger)
        {
            _config = config;
            _lookup = lookup;
            _importService = importService;
            _exportService = exportService;
            _logger = logger;
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
                        HttpUtility.UrlEncode(_config["CallBackUrl"]),
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
                ModelState.AddModelError("QueryParams.Name", "You must choose at least one search criteria.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            byte[] data;
            using (var br = new BinaryReader(QueryParams.ImportFile.OpenReadStream()))
                data = br.ReadBytes((int)QueryParams.ImportFile.OpenReadStream().Length);

            ByteArrayContent bytes = new ByteArrayContent(data);

            MultipartFormDataContent multiContent = new MultipartFormDataContent();

            multiContent.Add(bytes, "file", QueryParams.ImportFile.FileName);
            multiContent.Add(new StringContent(QueryParams.Email.ToString()), "Email");
            multiContent.Add(new StringContent(QueryParams.Name.ToString()), "Name");
            multiContent.Add(new StringContent(QueryParams.Company.ToString()), "Company");
            multiContent.Add(new StringContent(QueryParams.Obu), "Obu");
            multiContent.Add(new StringContent(QueryParams.Name.ToString()), "QueryObjectId");
            multiContent.Add(new StringContent(SalesForceCredential.InstanceUrl), "InstanceUrl");
            multiContent.Add(new StringContent(SalesForceCredential.Token), "Token");
            multiContent.Add(new StringContent(SalesForceCredential.ApiVersion), "ApiVersion");


            if (QueryParams.ImportFile.Length > 0)
            {
                var results = await httpClient.PostAsync("http://localhost:7071/api/Function2", multiContent);
                //Accounts = await _importService.Import(QueryParams, SalesForceCredential);
                return _exportService.ExportResults(Accounts, Path.GetFileNameWithoutExtension(QueryParams.ImportFile.FileName));
            }

            var lookupPerson = new Person();
            Accounts = await _lookup.LookupContact(QueryParams, lookupPerson, SalesForceCredential);
            return Page();
        }

    }
}
