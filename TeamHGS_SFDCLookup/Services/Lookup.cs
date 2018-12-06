using System.Collections.Generic;
using System.Linq;
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

            var lookupEmail = lookupPerson.LookupEmail.Replace("'", @"\'");

            var query = "SELECT id, name, email, AccountId, Account.Name, Direct_Phone__c, Email_Invalid__c, HasOptedOutOfEmail, Industry_Vertical__c, LeadSource, Title, Description, Originating_Business_Unit__c FROM Contact";
            var useAnd = false;
            if (queryParams != null)
            {
                if (queryParams.Name)
                {
                    useAnd = true;
                    query = $"{query} WHERE name LIKE '%{lookupPerson.LookupName}%'";
                }

                if (queryParams.Email)
                {
                    if (useAnd)
                    {
                        query = $"{query} AND email LIKE '%{lookupEmail}%'";
                    }
                    else
                    {
                        useAnd = true;
                        query = $"{query} WHERE email LIKE '%{lookupEmail}%'";
                    }

                }

                if (queryParams.Company)
                {
                    if (useAnd)
                    {
                        query = $"{query} AND Account.Name LIKE '%{lookupPerson.LookupAccountName}%'";
                    }
                    else
                    {
                        useAnd = true;
                        query = $"{query} WHERE Account.Name LIKE '%{lookupPerson.LookupAccountName}%'";
                    }

                }

                if (!string.IsNullOrWhiteSpace(queryParams.Obu))
                {
                    if (useAnd)
                    {
                        query = $"{query} AND Originating_Business_Unit__c = '{queryParams.Obu}'";
                    }
                    else
                    {
                        //useAnd = true;
                        query = $"{query} WHERE Originating_Business_Unit__c = '{queryParams.Obu}'";
                    }

                }
            }

            query = $"{query} ORDER BY name";
            var contacts = await client.QueryAsync<Person>(query);
            foreach (var c in contacts.Records)
            {
                if (string.IsNullOrWhiteSpace(c.AccountId)) continue;

                var accountQuery = $"SELECT id, name FROM Account WHERE id = '{c.AccountId}'";
                var account = await client.QueryAsync<Account>(accountQuery);
                c.AccountName = account.Records.First().Name;
            }
            return contacts.Records;
        }
    }
}
