using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public async Task<List<Person>> Import(IFormFile file, QueryParameters queryParameters, SalesForceCredential sfdcCredential)
        {
            //Get file
            var newfile = new FileInfo(file.FileName);

            //Establish import and error objects
            var importObj = new List<Person>();
            var errorObj = new List<Person>();

            //Check if file is an Excel File
            if (!newfile.Extension.Contains(".xls")) throw new Exception("File must be in Excel format.");

            //Create an excel package
            using (var package = new ExcelPackage(file.OpenReadStream()))
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
                        Email = worksheet.Cells[row, emailColumn].Value.ToString(),
                        AccountName = worksheet.Cells[row, accountColumn].Value.ToString()
                    };

                    if (searchByFullName)
                    {
                        newPerson.Name = worksheet.Cells[row, nameColumn].Value.ToString();
                    }
                    else
                    {
                        newPerson.First = worksheet.Cells[row, firstNameColumn].Value.ToString();
                        newPerson.Last = worksheet.Cells[row, lastNameColumn].Value.ToString();
                    }

                    //Lookup this person in Salesforce
                    var personResult = await _lookup.LookupContact(queryParameters, sfdcCredential);


                } // End For Loop
                return errorObj;
            }
        }

    }
}
