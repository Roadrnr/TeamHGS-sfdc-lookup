using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamHGS_SFDCLookup.Models
{
    public class QueryParameters
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public int OBU { get; set; }
        public bool ByEmail { get; set; }
        public bool ByName { get; set; }
        public bool ByCompany { get; set; }

    }
}
