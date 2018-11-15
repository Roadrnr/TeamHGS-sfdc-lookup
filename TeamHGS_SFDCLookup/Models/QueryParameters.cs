namespace TeamHGS_SFDCLookup.Models
{
    public class QueryParameters
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public int ObuId { get; set; }
        public int QueryObjectId { get; set; }
        public Enums.QueryObject QueryObject { get; set; }

    }
}
