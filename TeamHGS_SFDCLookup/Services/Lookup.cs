using System.Collections.Generic;
using System.Threading.Tasks;
using TeamHGS_SFDCLookup.Models;

namespace TeamHGS_SFDCLookup.Services
{
    public class Lookup : ILookup
    {
        public async Task<List<Person>> LookupContact(QueryParameters queryParams, SalesForceCredential sfdCredential)
        {

            var client = new ForceClient(sfdCredential.InstanceUrl, sfdCredential.Token,
                sfdCredential.ApiVersion);


            var query = "SELECT id, name, email, Account.Name, Originating_Business_Unit__c FROM Contact";
            var useAnd = false;
            if (!string.IsNullOrWhiteSpace(queryParams.Name))
            {
                useAnd = true;
                query = $"{query} WHERE name LIKE '%{queryParams.Name}%'";
            }

            if (!string.IsNullOrWhiteSpace(queryParams.Email))
            {
                if (useAnd)
                {
                    query = $"{query} AND email LIKE '%{queryParams.Email}%'";
                }
                else
                {
                    useAnd = true;
                    query = $"{query} WHERE email LIKE '%{queryParams.Email}%'";
                }

            }

            if (!string.IsNullOrWhiteSpace(queryParams.Company))
            {
                if (useAnd)
                {
                    query = $"{query} AND Account.Name LIKE '%{queryParams.Company}%'";
                }
                else
                {
                    useAnd = true;
                    query = $"{query} WHERE Account.Name LIKE '%{queryParams.Company}%'";
                }

            }

            if (queryParams.ObuId > 0)
            {
                if (useAnd)
                {
                    query = $"{query} AND Originating_Business_Unit__c LIKE '%{queryParams.Company}%'";
                }
                else
                {
                    //useAnd = true;
                    query = $"{query} WHERE Originating_Business_Unit__c LIKE '%{queryParams.Company}%'";
                }

            }

            query = $"{query} ORDER BY name";
            var accounts = await client.QueryAsync<Person>(query);
            return accounts.Records;
        }
    }
}
