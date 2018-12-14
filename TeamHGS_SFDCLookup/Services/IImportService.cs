using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TeamHGS_SFDCLookup.Models;

namespace TeamHGS_SFDCLookup.Services
{
    public interface IImportService
    {
        Task<List<Person>> Import(QueryParameters queryParameters, SalesForceCredential sfdcCredential);
        Task<int> ImportBackgroundTest(CancellationToken token);
    }
}