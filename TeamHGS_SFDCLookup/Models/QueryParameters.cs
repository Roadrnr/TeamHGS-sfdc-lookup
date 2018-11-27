using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;

namespace TeamHGS_SFDCLookup.Models
{
    public class QueryParameters
    {
        public bool Name { get; set; }
        public bool Email { get; set; }
        public bool Company { get; set; }
        public int ObuId { get; set; }
        public int QueryObjectId { get; set; }
        public Enums.QueryObject QueryObject { get; set; }

        public IFormFile ImportFile { get; set; }
    }
}
