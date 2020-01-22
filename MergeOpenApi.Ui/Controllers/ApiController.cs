using MergeOpenApi.Ui.Model;
using Microsoft.AspNetCore.Mvc;

namespace MergeOpenApi.Ui.Controllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IGetSchemaCached _getSchemaCached;

        public ApiController(IGetSchemaCached getSchemaCached)
        {
            _getSchemaCached = getSchemaCached;
        }
        
        [HttpGet("/swagger.json")]
        public ActionResult Get()
        {
            var json = _getSchemaCached.Execute();
            if (json == null)
            {
                return NotFound();
            }
            
            return Content(json, "application/json");
        }
    }
}
