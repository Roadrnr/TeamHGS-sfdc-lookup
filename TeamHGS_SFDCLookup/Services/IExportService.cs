using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TeamHGS_SFDCLookup.Models;

namespace TeamHGS_SFDCLookup.Services
{
    public interface IExportService
    {
        FileContentResult ExportResults(List<Person> people, string fileName);
    }
}