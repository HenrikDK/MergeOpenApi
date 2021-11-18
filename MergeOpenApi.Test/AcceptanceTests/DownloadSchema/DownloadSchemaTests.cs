namespace MergeOpenApi.Test.AcceptanceTests.DownloadSchema;

[Story(AsA = "User", 
    IWant = "To download OpenApi schemas", 
    SoThat = "I can merge them into a combined schema")]
public class DownloadSchemaTests : Reporting
{
    [Test]
    public void DeployedServicesShouldHaveTheirSchemaDownloaded()
    {
        new DeployedServicesShouldHaveTheirSchemaDownloaded().BDDfy<DownloadSchemaTests>();
    }

    [Test]
    public void UnavailableServicesShouldRetry()
    {
        new UnavailableServicesShouldRetry().BDDfy<DownloadSchemaTests>();
    }
        
    [Test]
    public void UnavailableServicesExceedingMaxRetriesShouldBeDisabled()
    {
        new UnavailableServicesExceedingMaxRetriesShouldBeDisabled().BDDfy<DownloadSchemaTests>();
    }

    [Test]
    public void ServicesAlreadyDownloadedShouldNotDownloadAgain()
    {
        new ServicesAlreadyDownloadedShouldNotDownloadAgain().BDDfy<DownloadSchemaTests>();
    }
}