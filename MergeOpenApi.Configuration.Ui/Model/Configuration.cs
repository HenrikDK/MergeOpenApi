using System.ComponentModel.DataAnnotations;
using MergeOpenApi.Configuration.Ui.Model.Enums;

namespace MergeOpenApi.Configuration.Ui.Model
{
    public class Configuration
    {
        [Required(ErrorMessage = "Please specify a title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please specify an Api description")]
        public string Description { get; set; }
        
        public string TermsUrl { get; set; }
        public string ContactEmail { get; set; }
        
        public string LicenseName { get; set; }
        public string LicenseUrl { get; set; }
        
        public SecurityType? SecurityType { get; set; }
        public string SecurityKeyName { get; set; }
        
        [Required(ErrorMessage = "Please specify a url substring filter (eg. '/v1/') to select which endpoints are exposed in the combined Api")]
        public string UrlFilter { get; set; }
        
        [Required(ErrorMessage = "Please specify the path endpoint for the Api services json (eg. '/swagger.json')'")]
        public string JsonEndpoint { get; set; }
    }
}
