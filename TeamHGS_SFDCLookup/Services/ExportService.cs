using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
        public FileContentResult ExportResults(List<Person> people, string fileName)
        {

            fileName = $"{fileName}_Results.xlsx";
            var file = new FileInfo("Results.xlsx");

            // add a new worksheet to the empty workbook
            var package = new ExcelPackage(file);
            package.Workbook.Properties.Title = "Results";
            var worksheet = package.Workbook.Worksheets.Add("Results");
            //First add the headers
            var headerCells = worksheet.Cells[1, 1, 1, 16];
            var headerFont = headerCells.Style.Font;
            headerFont.Bold = true;
            var totalRowCount = people.Count + 1;

            //Set Header Column Names
            worksheet.Cells[1, 1].Value = "Id";
            worksheet.Cells[1, 2].Value = "Original Name";
            worksheet.Cells[1, 3].Value = "Found Name";
            worksheet.Cells[1, 4].Value = "Original Email";
            worksheet.Cells[1, 5].Value = "Found Email";
            worksheet.Cells[1, 6].Value = "Original Company";
            worksheet.Cells[1, 7].Value = "Found Company";
            worksheet.Cells[1, 8].Value = "OBU";
            worksheet.Cells[1, 9].Value = "Phone";
            worksheet.Cells[1, 10].Value = "Email Invalid?";
            worksheet.Cells[1, 11].Value = "Original Email Opt Out?";
            worksheet.Cells[1, 12].Value = "Found Email Opt Out?";
            worksheet.Cells[1, 13].Value = "Industry";
            worksheet.Cells[1, 14].Value = "Lead Source";
            worksheet.Cells[1, 15].Value = "Original Title";
            worksheet.Cells[1, 16].Value = "Found Title";

            //Set Column Widths
            worksheet.Column(1).Width = 20;
            worksheet.Column(2).Width = 20;
            worksheet.Column(3).Width = 20;
            worksheet.Column(4).Width = 30;
            worksheet.Column(5).Width = 30;
            worksheet.Column(6).Width = 20;
            worksheet.Column(7).Width = 20;
            worksheet.Column(8).Width = 20;
            worksheet.Column(9).Width = 20;
            worksheet.Column(10).Width = 15;
            worksheet.Column(11).Width = 15;
            worksheet.Column(12).Width = 15;
            worksheet.Column(13).Width = 20;
            worksheet.Column(14).Width = 30;
            worksheet.Column(15).Width = 30;
            worksheet.Column(16).Width = 30;

            //Set Column Formatting
            worksheet.Cells[$"B1:B{totalRowCount}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[$"D1:D{totalRowCount}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[$"F1:F{totalRowCount}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[$"K1:K{totalRowCount}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[$"O1:O{totalRowCount}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[$"B1:B{totalRowCount}"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            worksheet.Cells[$"D1:D{totalRowCount}"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            worksheet.Cells[$"F1:F{totalRowCount}"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            worksheet.Cells[$"K1:K{totalRowCount}"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            worksheet.Cells[$"O1:O{totalRowCount}"].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            worksheet.Column(10).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Column(11).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Column(12).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            //Add values
            var rowNumber = 2;
            //var createResultViewModels = results as IList<CreateResultViewModel> ?? results.ToList();
            for (var i = 0; i < people.Count(); i++)
            {
                var currentResult = people.ElementAt(i);
                worksheet.Cells["A" + rowNumber].Value = currentResult.Id;
                worksheet.Cells["B" + rowNumber].Value = currentResult.LookupName;
                worksheet.Cells["C" + rowNumber].Value = currentResult.Name;
                worksheet.Cells["D" + rowNumber].Value = currentResult.LookupEmail;
                worksheet.Cells["E" + rowNumber].Value = currentResult.Email;
                worksheet.Cells["F" + rowNumber].Value = currentResult.LookupAccountName;
                worksheet.Cells["G" + rowNumber].Value = currentResult.AccountName;
                worksheet.Cells["H" + rowNumber].Value = currentResult.Originating_Business_Unit__c;
                worksheet.Cells["I" + rowNumber].Value = currentResult.Direct_Phone__c;
                worksheet.Cells["J" + rowNumber].Value = currentResult.Email_Invalid__c ? "X" : "";
                worksheet.Cells["K" + rowNumber].Value = currentResult.LookupOptOut ? "X" : "";
                worksheet.Cells["L" + rowNumber].Value = currentResult.HasOptedOutOfEmail ? "X" : "";
                worksheet.Cells["M" + rowNumber].Value = currentResult.Industry_Vertical__c;
                worksheet.Cells["N" + rowNumber].Value = currentResult.LeadSource;
                worksheet.Cells["O" + rowNumber].Value = currentResult.LookupTitle;
                worksheet.Cells["P" + rowNumber].Value = currentResult.Title;
                rowNumber++;
            }

            //var modelTable = worksheet.Cells["A1:P" + rowNumber];

            // Assign borders
            //modelTable.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            //modelTable.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            //modelTable.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            //modelTable.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            //worksheet.View.FreezePanes(2, 12);
            worksheet.Cells["A1:P1"].AutoFilter = true;

            var resultFile = new FileContentResult(package.GetAsByteArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                FileDownloadName = fileName
            };

            return resultFile;
        }
    }
}
