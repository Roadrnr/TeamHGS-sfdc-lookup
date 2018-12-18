using Microsoft.AspNetCore.Mvc;
using TeamHGS_SFDCLookup.Models;

namespace TeamHGS_SFDCLookup.Services
{
    public interface IImportService
    {
        string Import(QueryParameters queryParameters, SalesForceCredential sfdcCredential);
    }
}