using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using Microsoft.Extensions.Primitives;
using TeamHGS_Functions.Models;
using System.Collections.Generic;
using OfficeOpenXml;
using Salesforce.Force;

namespace TeamHGS_Functions
{
    public static class Function1
    {

        private static HttpClient httpClient = new HttpClient();

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            var client = new ForceClient(sfdCredential.InstanceUrl, sfdCredential.Token,
                sfdCredential.ApiVersion);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }

        [FunctionName("Function2")]
        public static async Task<IActionResult> RunFunction2([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            var received = req.Form.TryGetValue("Token", out StringValues strToken);
            //req.Form.TryGetValue("files", out ByteArrayContent excelFile);
            var excelFile = req.Form.Files.GetFile("files");

            //Get file
            var newFile = new FileInfo(excelFile.FileName);

            //Establish import and error objects
            var importObj = new List<Person>();

            //Check if file is an Excel File
            if (!newFile.Extension.Contains(".xls")) throw new Exception("File must be in Excel format.");

            //Create an excel package
            using (var package = new ExcelPackage(excelFile.OpenReadStream()))
            {
                //Get the first worksheet in the file
                var worksheet = package.Workbook.Worksheets[1];

                //Set rowcount, colcount and variables for the required columns
                var rowCount = worksheet.Dimension.Rows;
                var colCount = worksheet.Dimension.Columns;
                var emailColumn = 0;
                var companyColumn = 0;
                var firstNameColumn = 0;
                var lastNameColumn = 0;
                var nameColumn = 0;
                var titleColumn = 0;
                var unsubColumn = 0;
                var searchByFullName = false;
                //Loop through columns to identify the column number for each of the required columns
                for (var col = 1; col <= colCount; col++)
                {
                    var columnValue = Convert.ToString(worksheet.Cells[1, col].Value);
                    if (columnValue.ToLower().Contains("first"))
                    {
                        firstNameColumn = col;
                    }
                    else if (columnValue.ToLower().Contains("last"))
                    {
                        lastNameColumn = col;
                    }
                    else if (columnValue.ToLower().Contains("name"))
                    {
                        nameColumn = col;
                        searchByFullName = true;
                    }
                    else if (columnValue.ToLower().Contains("email"))
                    {
                        emailColumn = col;
                    }
                    else if (columnValue.ToLower().Contains("company"))
                    {
                        companyColumn = col;
                    }
                    else if (columnValue.ToLower().Contains("title"))
                    {
                        titleColumn = col;
                    }
                    else if (columnValue.ToLower().Contains("unsubscribe"))
                    {
                        unsubColumn = col;
                    }
                }

                //Loop through the rows of the worksheet, skipping row 1 (header row)
                for (var row = 2; row <= rowCount; row++)
                {
                    //If first field is blank exit the for loop
                    if (worksheet.Cells[row, 1].Value == null)
                    {
                        break;
                    }

                    //Create temporary person object
                    var lookupPerson = new Person
                    {
                        LookupEmail = worksheet.Cells[row, emailColumn].Value != null ? worksheet.Cells[row, emailColumn].Value.ToString() : "",
                        LookupAccountName = worksheet.Cells[row, companyColumn].Value == null ? "" : worksheet.Cells[row, companyColumn].Value.ToString()
                    };

                    if (titleColumn > 0)
                        lookupPerson.LookupTitle = worksheet.Cells[row, titleColumn].Value == null
                            ? ""
                            : worksheet.Cells[row, titleColumn].Value.ToString();
                    if (unsubColumn > 0) lookupPerson.LookupOptOut = worksheet.Cells[row, unsubColumn].Value != null;

                    if (searchByFullName)
                    {
                        lookupPerson.LookupName = worksheet.Cells[row, nameColumn].Value != null ? worksheet.Cells[row, nameColumn].Value.ToString() : "";
                    }
                    else
                    {
                        lookupPerson.LookupFirst = worksheet.Cells[row, firstNameColumn].Value != null ? worksheet.Cells[row, firstNameColumn].Value.ToString() : "";
                        lookupPerson.LookupLast = worksheet.Cells[row, lastNameColumn].Value != null ? worksheet.Cells[row, lastNameColumn].Value.ToString() : "";
                        lookupPerson.LookupName = $"{lookupPerson.LookupFirst} {lookupPerson.LookupLast}";
                    }

                    var response = await httpClient.PostAsync("http://localhost:7071/api/Function1");
                    var jsonResponse = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());

                    var personResult = await _lookup.LookupContact(queryParameters, lookupPerson, sfdcCredential);

                    if (personResult != null && personResult.Count > 0)
                    {
                        foreach (var found in personResult)
                        {
                            var obu = string.IsNullOrWhiteSpace(found.Originating_Business_Unit__c)
                                ? "No OBU"
                                : found.Originating_Business_Unit__c;

                            var foundPerson = new Person
                            {
                                Name = found.Name,
                                Email = found.Email,
                                AccountId = $"{found.AccountId}",
                                AccountName = $"{found.AccountName} ({found.AccountId})",
                                CompanyNameMatchCount = personResult.Count,
                                Id = $"{found.Id}",
                                Originating_Business_Unit__c = $"{obu}",
                                Direct_Phone__c = $"{found.Direct_Phone__c}",
                                Email_Invalid__c = found.Email_Invalid__c,
                                HasOptedOutOfEmail = found.HasOptedOutOfEmail,
                                Industry_Vertical__c = $"{found.Industry_Vertical__c}",
                                LeadSource = $"{found.LeadSource}",
                                Title = $"{found.Title}",
                                Description = $"{found.Description}",
                                LookupName = lookupPerson.LookupName,
                                LookupFirst = lookupPerson.LookupFirst,
                                LookupLast = lookupPerson.LookupLast,
                                LookupEmail = lookupPerson.LookupEmail,
                                LookupAccountName = lookupPerson.LookupAccountName,
                                LookupTitle = lookupPerson.LookupTitle,
                                LookupOptOut = lookupPerson.LookupOptOut
                            };

                            importObj.Add(foundPerson);
                        }
                    }
                    else
                    {
                        var notFoundPerson = new Person
                        {
                            Name = lookupPerson.Name,
                            Email = lookupPerson.Email,
                            Id = "NOT FOUND",
                            CompanyNameMatch = false,
                            NameMatch = false,
                            EmailMatch = false,
                            AccountId = "",
                            AccountName = lookupPerson.AccountName,
                            CompanyNameMatchCount = 0,
                            Originating_Business_Unit__c = "NONE",
                            Direct_Phone__c = "",
                            Email_Invalid__c = false,
                            HasOptedOutOfEmail = lookupPerson.HasOptedOutOfEmail,
                            Industry_Vertical__c = "",
                            LeadSource = "",
                            Title = lookupPerson.Title,
                            Description = "",
                            LookupName = lookupPerson.LookupName,
                            LookupFirst = lookupPerson.LookupFirst,
                            LookupLast = lookupPerson.LookupLast,
                            LookupEmail = lookupPerson.LookupEmail,
                            LookupAccountName = lookupPerson.LookupAccountName,
                            LookupTitle = lookupPerson.LookupTitle,
                            LookupOptOut = lookupPerson.LookupOptOut
                        };
                        importObj.Add(notFoundPerson);
                    }
                } // End For Loop
                return importObj;
            }

            var response = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/todos/1");
            var jsonResponse = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
            await Task.Delay(1000);
            return new OkObjectResult($"Function 2 Ran - Response: {jsonResponse}");
        }

        private void ProcessFile(IFormFile file)
        {
            
        }
    }
}
