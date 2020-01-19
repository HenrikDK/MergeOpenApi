using MergeOpenApi.Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace MergeOpenApi.Api.Controllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly IGetMergedSchema _getMergedSchema;
        private readonly ISaveServiceDeployment _saveServiceDeployment;

        public ApiController(IGetMergedSchema getMergedSchema, ISaveServiceDeployment saveServiceDeployment)
        {
            _getMergedSchema = getMergedSchema;
            _saveServiceDeployment = saveServiceDeployment;
        }

        /// <summary>
        /// Endpoint to use with service deployment to ensure current merged schema reflects all MergeOpenApi services.
        /// </summary>
        /// <param name="serviceName">Name of the deployed service to update</param>
        /// <param name="urls">Comma separated list of the deployed services urls that are reachable by the MergeOpenApi service</param>
        /// <returns></returns>
        [HttpPost("/deploy")]
        public ActionResult Post([FromForm] string serviceName, [FromForm] string urls)
        {
            if (string.IsNullOrWhiteSpace(serviceName) || string.IsNullOrEmpty(serviceName))
            {
                return StatusCode(400);
            }

            if (string.IsNullOrWhiteSpace(urls) || string.IsNullOrEmpty(urls))
            {
                return StatusCode(400);
            }
            
            _saveServiceDeployment.Execute(serviceName, urls);
            
            return Ok();
        }

        /// <summary>
        /// Get the current merged schema
        /// </summary>
        /// <returns>merged Open Api document</returns>
        [HttpGet("/merged")]
        public ActionResult<string> Get()
        {
            var json = _getMergedSchema.Execute();

            if (json == null)
            {
                return NotFound();
            }
            
            return Ok(json);
        }
    }
}
