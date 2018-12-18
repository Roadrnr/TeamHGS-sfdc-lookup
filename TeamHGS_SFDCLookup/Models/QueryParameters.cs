using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TeamHGS_SFDCLookup.Models
{
    public class QueryParameters
    {
        public bool Name { get; set; }
        public bool Email { get; set; }
        public bool Company { get; set; }
        public string Obu { get; set; }
        public int QueryObjectId { get; set; }
        public Enums.QueryObject QueryObject { get; set; }
        [Required]
        public IFormFile ImportFile { get; set; }
        public string ImportFileUrl { get; set; }
    }
}
