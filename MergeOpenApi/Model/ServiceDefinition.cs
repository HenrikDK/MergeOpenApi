using MergeOpenApi.Model.Enums;

namespace MergeOpenApi.Model;

public class ServiceDefinition
{
    public int Id { get; set; }
    public string JsonData { get; set; }
    public ServiceStatus Status { get; set; }
    public int Retry { get; set; }
    public string ServiceUrls { get; set; }
}