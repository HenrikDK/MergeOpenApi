using MergeOpenApi.Model.Enums;

namespace MergeOpenApi.Model
{
    public class Configuration
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string TermsUrl { get; set; }
        public string ContactEmail { get; set; }
        public string LicenseName { get; set; }
        public string LicenseUrl { get; set; }
        public SecurityType SecurityType { get; set; }
        public string SecurityKeyName { get; set; }
        public string UrlFilter { get; set; }
        public string JsonEndpoint { get; set; }
    }
}
