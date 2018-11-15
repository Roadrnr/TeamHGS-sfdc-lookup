using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamHGS_SFDCLookup.Models
{
    public class Person
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string Description { get; set; }
        public string Email { get; set; }
        public string AccountName { get; set; }
        public string Originating_Business_Unit__c { get; set; }
        public string AccountId { get; set; }

        public bool NameMatch { get; set; }
        public bool EmailMatch { get; set; }
        public bool CompanyNameMatch { get; set; }
        public string SfdcId { get; set; }
        public int NameMatchCount { get; set; }
        public int EmailMatchCount { get; set; }
        public int CompanyNameMatchCount { get; set; }

    }
}
