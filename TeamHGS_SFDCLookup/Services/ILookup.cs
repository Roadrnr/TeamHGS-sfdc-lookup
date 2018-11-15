using System.Collections.Generic;
using System.Threading.Tasks;
using TeamHGS_SFDCLookup.Models;

namespace TeamHGS_SFDCLookup.Services
{
    public interface ILookup
    {
        Task<List<Person>> LookupContact(QueryParameters queryParams, SalesForceCredential sfdCredential);
    }
}