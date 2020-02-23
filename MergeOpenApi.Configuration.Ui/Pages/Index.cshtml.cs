using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using MergeOpenApi.Configuration.Ui.Model.Commands;
using MergeOpenApi.Configuration.Ui.Model.Enums;
using MergeOpenApi.Configuration.Ui.Model.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace MergeOpenApi.Configuration.Ui.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IGetConfiguration _getConfiguration;
        private readonly IUpdateServicesToTriggerMerge _updateServicesToTriggerMerge;
        private readonly ISaveConfiguration _saveConfiguration;
        
        [BindProperty(SupportsGet = true)]
        public IList<SelectListItem> SecurityTypes { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public Model.Configuration Configuration { get; set; }

        public IndexModel(ILogger<IndexModel> logger,
            IGetConfiguration getConfiguration,
            IUpdateServicesToTriggerMerge updateServicesToTriggerMerge,
            ISaveConfiguration saveConfiguration)
        {
            _logger = logger;
            _getConfiguration = getConfiguration;
            _updateServicesToTriggerMerge = updateServicesToTriggerMerge;
            _saveConfiguration = saveConfiguration;
        }

        public void OnGet()
        {
            SecurityTypes = Enum.GetValues(typeof(SecurityType)).Cast<SecurityType>().Select(x => new SelectListItem(x.ToString(), ((int)x).ToString())).ToList();
            var configuration = _getConfiguration.Execute();
            if (configuration == null)
            {
                Configuration = new Model.Configuration
                {
                    UrlFilter = "/",
                    JsonEndpoint = "/swagger.json"
                };
                return;
            }

            Configuration = configuration;
        }

        public ActionResult OnPost()
        {
            if (Configuration.SecurityType == SecurityType.ApiKey &&
                (string.IsNullOrEmpty(Configuration.SecurityKeyName) || string.IsNullOrWhiteSpace(Configuration.SecurityKeyName)))
            {
                ModelState.AddModelError("SecurityKeyName", "ApiKey requires a name");
            }

            if (!string.IsNullOrWhiteSpace(Configuration.LicenseName) &&
                (string.IsNullOrEmpty(Configuration.LicenseUrl) || string.IsNullOrWhiteSpace(Configuration.LicenseUrl)))
            {
                ModelState.AddModelError("LicenseUrl", "License requires a url");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (var scope = new TransactionScope())
            {
                _saveConfiguration.Execute(Configuration);
                _updateServicesToTriggerMerge.Execute();
            
                scope.Complete();
            }

            return RedirectToPage("Index");
        }
    }
}
