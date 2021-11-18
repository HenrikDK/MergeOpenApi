using MergeOpenApi.Api.Model;

namespace MergeOpenApi.Api.Controllers;

[ApiController]
public class ApiController : ControllerBase
{
    private readonly ISaveServiceDeployment _saveServiceDeployment;

    public ApiController(ISaveServiceDeployment saveServiceDeployment)
    {
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
}