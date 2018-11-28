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
        public async Task<List<Person>> Import(QueryParameters queryParameters, SalesForceCredential sfdcCredential)
        {
            //Get file
            var newfile = new FileInfo(queryParameters.ImportFile.FileName);

            //Establish import and error objects
            var importObj = new List<Person>();
            //var errorObj = new List<Person>();

            //Check if file is an Excel File
            if (!newfile.Extension.Contains(".xls")) throw new Exception("File must be in Excel format.");

            //Create an excel package
            using (var package = new ExcelPackage(queryParameters.ImportFile.OpenReadStream()))
            {
                //Get the first worksheet in the file
                var worksheet = package.Workbook.Worksheets.First();

                //Set rowcount, colcount and variables for the required columns
                var rowCount = worksheet.Dimension.Rows;
                var colCount = worksheet.Dimension.Columns;
                var emailColumn = 0;
                var accountColumn = 0;
                var firstNameColumn = 0;
                var lastNameColumn = 0;
                var nameColumn = 0;
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
                    else if (columnValue.ToLower().Contains("account"))
                    {
                        accountColumn = col;
                    }
                }

                //Loop through the rows of the worksheet, skipping row 1 (header row)
                for (var row = 2; row <= rowCount; row++)
                {
                    //If Event field is blank exit the for loop
                    var strValue = worksheet.Cells[row, 1].Value.ToString();
                    if (string.IsNullOrWhiteSpace(strValue))
                    {
                        break;
                    }
                    
                    //Create temporary result object
                    var newPerson = new Person
                    {
                        Email = worksheet.Cells[row, emailColumn].Value != null ? worksheet.Cells[row, emailColumn].Value.ToString() : "",
                        AccountName = worksheet.Cells[row, accountColumn].Value == null ? "" : worksheet.Cells[row, accountColumn].Value.ToString()
                    };

                    if (searchByFullName)
                    {
                        newPerson.Name = worksheet.Cells[row, nameColumn].Value != null ? worksheet.Cells[row, nameColumn].Value.ToString() : "";
                    }
                    else
                    {
                        newPerson.First = worksheet.Cells[row, firstNameColumn].Value != null ? worksheet.Cells[row, firstNameColumn].Value.ToString(): "";
                        newPerson.Last = worksheet.Cells[row, lastNameColumn].Value != null ? worksheet.Cells[row, lastNameColumn].Value.ToString() : "";
                        newPerson.Name = $"{newPerson.First} {newPerson.Last}";
                    }

                    //Lookup this person in Salesforce
                    var lookupPerson = new Person
                    {
                        Name = newPerson.Name,
                        Email = newPerson.Email,
                        AccountName = newPerson.AccountName
                    };

                    var personResult = await _lookup.LookupContact(queryParameters, lookupPerson, sfdcCredential);

                    if (personResult != null && personResult.Count > 0)
                    {
                        if (personResult.Count == 1)
                        {
                            newPerson.AccountId += personResult.First().AccountId;
                            newPerson.AccountName += personResult.First().AccountName;
                            newPerson.CompanyNameMatchCount = personResult.Count;
                            newPerson.Id = personResult.First().Id;
                            newPerson.Originating_Business_Unit__c = personResult.First().Originating_Business_Unit__c;
                            newPerson.Direct_Phone__c = personResult.First().Direct_Phone__c;
                            newPerson.Email_Invalid__c = personResult.First().Email_Invalid__c;
                            newPerson.HasOptedOutOfEmail = personResult.First().HasOptedOutOfEmail;
                            newPerson.Industry_Vertical__c = personResult.First().Industry_Vertical__c;
                            newPerson.LeadSource = personResult.First().LeadSource;
                            newPerson.Title = personResult.First().Title;
                            newPerson.Description = personResult.First().Description;
                        }
                        else
                        {
                            foreach (var found in personResult)
                            {
                                newPerson.AccountId += $"{found.AccountId}<br/>";
                                newPerson.AccountName += $"{found.AccountName} ({found.AccountId})<br/>";
                                newPerson.CompanyNameMatchCount = personResult.Count;
                                newPerson.Id += $"{found.Id}<br/>";
                                newPerson.Originating_Business_Unit__c += $"{found.Originating_Business_Unit__c}<br/>";
                                newPerson.Direct_Phone__c += $"{found.Direct_Phone__c}<br/>";
                                newPerson.Email_Invalid__c = found.Email_Invalid__c;
                                newPerson.HasOptedOutOfEmail = found.HasOptedOutOfEmail;
                                newPerson.Industry_Vertical__c += $"{found.Industry_Vertical__c}<br/>";
                                newPerson.LeadSource += $"{found.LeadSource}<br/>";
                                newPerson.Title += $"{found.Title}<br/>";
                                newPerson.Description += $"{found.Description}<br/>";
                            }
                        }
                    }
                    else
                    {
                        newPerson.Id = "NOT FOUND";
                        newPerson.CompanyNameMatch = false;
                        newPerson.NameMatch = false;
                        newPerson.EmailMatch = false;
                    }

                    importObj.Add(newPerson);

                } // End For Loop

                return importObj;
            }
        }

    }
}
