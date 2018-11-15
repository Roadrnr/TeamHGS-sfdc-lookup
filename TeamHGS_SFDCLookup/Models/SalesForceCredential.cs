namespace TeamHGS_SFDCLookup.Models
{
    public class SalesForceCredential
    {
        public string InstanceUrl { get; set; }
        public string RefreshToken { get; set; }
        public string Token { get; set; }
        public string ApiVersion { get; set; }
    }
}
