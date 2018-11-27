using System.Collections.Generic;
using System.Threading.Tasks;
using TeamHGS_SFDCLookup.Models;

namespace TeamHGS_SFDCLookup.Services
{
    public class Lookup : ILookup
    {
        public async Task<List<Person>> LookupContact(QueryParameters queryParams, Person lookupPerson, SalesForceCredential sfdCredential)
        {

            var client = new ForceClient(sfdCredential.InstanceUrl, sfdCredential.Token,
                sfdCredential.ApiVersion);


            var query = "SELECT id, name, email, Account.Name, Direct_Phone__c, Email_Invalid__c, HasOptedOutOfEmail, Industry_Vertical__c, LeadSource, Title, Description, Originating_Business_Unit__c FROM Contact";
            var useAnd = false;
            if (queryParams != null)
            {
                if (queryParams.Name)
                {
                    useAnd = true;
                    query = $"{query} WHERE name LIKE '%{lookupPerson.Name}%'";
                }

                if (queryParams.Email)
                {
                    if (useAnd)
                    {
                        query = $"{query} AND email LIKE '%{lookupPerson.Email}%'";
                    }
                    else
                    {
                        useAnd = true;
                        query = $"{query} WHERE email LIKE '%{lookupPerson.Email}%'";
                    }

                }

                if (queryParams.Company)
                {
                    if (useAnd)
                    {
                        query = $"{query} AND Account.Name LIKE '%{lookupPerson.AccountName}%'";
                    }
                    else
                    {
                        useAnd = true;
                        query = $"{query} WHERE Account.Name LIKE '%{lookupPerson.AccountName}%'";
                    }

                }

                if (queryParams.ObuId > 0)
                {
                    if (useAnd)
                    {
                        query = $"{query} AND Originating_Business_Unit__c LIKE '%{queryParams.ObuId}%'";
                    }
                    else
                    {
                        //useAnd = true;
                        query = $"{query} WHERE Originating_Business_Unit__c LIKE '%{queryParams.ObuId}%'";
                    }

                }
            }

            query = $"{query} ORDER BY name";
            var accounts = await client.QueryAsync<Person>(query);
            return accounts.Records;
        }
    }
}
