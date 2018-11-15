using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamHGS_SFDCLookup.Models
{
    public class IndexViewModel
    {
        public int OBU { get; set; }
        public Enums.Obu OBUs { get; set; }

        public bool ByName { get; set; }
        public bool ByEmail { get; set; }
        public bool ByCompany { get; set; }
    }
}
