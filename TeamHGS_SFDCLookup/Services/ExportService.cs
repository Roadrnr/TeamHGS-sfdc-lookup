using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using TeamHGS_SFDCLookup.Models;

namespace TeamHGS_SFDCLookup.Services
{
    public class ExportService : IExportService
    {

        /// <summary>
        /// Export results to an excel (.xlsx) file
        /// </summary>
        /// <returns>An excel (.xlsx) file containing results of the contact lookup in Salesforce.</returns>
        public FileContentResult ExportResults(List<Person> people)
        {

            var fileName = "Results.xlsx";
            var file = new FileInfo("Results.xlsx");

            // add a new worksheet to the empty workbook
            var package = new ExcelPackage(file);
            package.Workbook.Properties.Title = "Results";
            var worksheet = package.Workbook.Worksheets.Add("Results");
            //First add the headers
            var headerCells = worksheet.Cells[1, 1, 1, 11];
            var headerFont = headerCells.Style.Font;
            headerFont.Bold = true;

            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Name";
            worksheet.Cells[1, 3].Value = "Email";
            //worksheet.Column(3).Width = 30;
            //worksheet.Column(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //worksheet.Column(2).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 4].Value = "Company";
            //worksheet.Column(4).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 5].Value = "OBU";
            worksheet.Cells[1, 6].Value = "Phone";
            //worksheet.Column(6).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 7].Value = "Email Invalid?";
            //worksheet.Column(7).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 8].Value = "Email Opt Out?";
            //worksheet.Column(8).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[1, 9].Value = "Industry";
            //worksheet.Column(9).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //worksheet.Column(9).Width = 17;
            worksheet.Cells[1, 10].Value = "Lead Source";
            worksheet.Cells[1, 11].Value = "Title";
            //Add values
            var rowNumber = 2;
            //var createResultViewModels = results as IList<CreateResultViewModel> ?? results.ToList();
            for (var i = 0; i < people.Count(); i++)
            {
                var currentResult = people.ElementAt(i);
                worksheet.Cells["A" + rowNumber].Value = currentResult.Id;
                worksheet.Cells["B" + rowNumber].Value = currentResult.Name;
                worksheet.Cells["C" + rowNumber].Value = currentResult.Email;
                worksheet.Cells["D" + rowNumber].Value = currentResult.AccountName;
                worksheet.Cells["E" + rowNumber].Value = currentResult.Originating_Business_Unit__c;
                worksheet.Cells["F" + rowNumber].Value = currentResult.Direct_Phone__c;
                worksheet.Cells["G" + rowNumber].Value = currentResult.Email_Invalid__c ? "X" : "";
                worksheet.Cells["H" + rowNumber].Value = currentResult.HasOptedOutOfEmail ? "X" : "";
                worksheet.Cells["I" + rowNumber].Value = currentResult.Industry_Vertical__c;
                worksheet.Cells["J" + rowNumber].Value = currentResult.LeadSource;
                worksheet.Cells["K" + rowNumber].Value = currentResult.Title;
                rowNumber++;
            }

            var modelTable = worksheet.Cells["A1:K" + rowNumber];

            // Assign borders
            //modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            //modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            //modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.View.FreezePanes(2, 12);
            worksheet.Cells["A1:K1"].AutoFilter = true;

            var resultFile = new FileContentResult(package.GetAsByteArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = fileName
            };

            return resultFile;
        }
    }
}
