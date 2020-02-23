using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Transactions;
using MergeOpenApi.Configuration.Ui.Model;
using MergeOpenApi.Configuration.Ui.Model.Commands;
using MergeOpenApi.Configuration.Ui.Model.Enums;
using MergeOpenApi.Configuration.Ui.Model.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MoreLinq.Extensions;

namespace MergeOpenApi.Configuration.Ui.Pages
{
    public class ListModel : PageModel
    {
        private readonly IGetServices _getServices;
        private readonly IUpdateServicesToTriggerMerge _updateServicesToTriggerMerge;
        private readonly IUpdateServiceStatus _updateServiceStatus;
        
        [BindProperty(SupportsGet = true)]
        public IList<Service> Services { get; set; }
        
        public ListModel(IGetServices getServices,
            IUpdateServicesToTriggerMerge updateServicesToTriggerMerge,
            IUpdateServiceStatus updateServiceStatus)
        {
            _getServices = getServices;
            _updateServicesToTriggerMerge = updateServicesToTriggerMerge;
            _updateServiceStatus = updateServiceStatus;
        }

        public void OnGet()
        {
            Services = _getServices.Execute();
            
            Services.ForEach(x => x.Enabled = x.Status != ServiceStatus.Disabled);
        }

        public ActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var services = _getServices.Execute();

            services.ForEach(x => x.Enabled = x.Status != ServiceStatus.Disabled);

            var updates = Services.Where(x => !services.Any(s => x.Id == s.Id && x.Enabled == s.Enabled)).ToList();

            if (updates.Count == 0)
            {
                return RedirectToPage("List");
            }

            updates.ForEach(x => x.Status = x.Enabled ? ServiceStatus.Deployed : ServiceStatus.Disabled);

            using (var scope = new TransactionScope())
            {
                _updateServiceStatus.Execute(updates);
                _updateServicesToTriggerMerge.Execute();
                
                scope.Complete();
            }

            return RedirectToPage("List");
        }
    }
}
