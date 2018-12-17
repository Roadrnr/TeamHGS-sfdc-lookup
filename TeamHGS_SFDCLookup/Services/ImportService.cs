using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using TeamHGS_SFDCLookup.Models;

namespace TeamHGS_SFDCLookup.Services
{
    public class ImportService : IImportService
    {
        private readonly ILookup _lookup;

        public ImportService(ILookup lookup)
        {
            _lookup = lookup;
        }
        public List<Person> Import(QueryParameters queryParameters, SalesForceCredential sfdcCredential)
        {
            //Get file
            var newFile = new FileInfo(queryParameters.ImportFile.FileName);

            //Establish import and error objects
            var importObj = new List<Person>();

            //Check if file is an Excel File
            if (!newFile.Extension.Contains(".xls")) throw new Exception("File must be in Excel format.");

            //Create an excel package
            using (var package = new ExcelPackage(queryParameters.ImportFile.OpenReadStream()))
            {
                //Get the first worksheet in the file
                var worksheet = package.Workbook.Worksheets.First();

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

                    var personResult = Task.Run(async() => 
                        await _lookup.LookupContact(queryParameters, lookupPerson, sfdcCredential)).Result;

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
        }
    }
}
