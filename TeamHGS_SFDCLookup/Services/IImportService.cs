using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TeamHGS_SFDCLookup.Models;

namespace TeamHGS_SFDCLookup.Services
{
    public interface IImportService
    {
        Task<List<Person>> Import(IFormFile file, QueryParameters queryParameters, SalesForceCredential sfdcCredential);
    }
}