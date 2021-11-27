namespace MergeOpenApi.Test.AcceptanceTests.MergeSchema;

[Story(AsA = "User", 
    IWant = "To merge OpenApi schemas", 
    SoThat = "I can present a combined Api overview")]
public class MergeSchemaTests : Reporting
{
    [Test]
    public void ASingleServiceShouldNotBeMerged()
    {
        new ASingleServiceShouldNotBeMerged().BDDfy<MergeSchemaTests>();
    }

    [Test]
    public void MultipleServiceSchemasShouldBeMerged()
    {
        new MultipleServiceSchemasShouldBeMerged().BDDfy<MergeSchemaTests>();
    }
        
    [Test]
    public void MergedServiceSchemasShouldHaveTheirTypesRenamed()
    {
        new MergedServiceSchemasShouldHaveTheirTypesRenamed().BDDfy<MergeSchemaTests>();
    }
        
    [Test]
    public void NonPublicUrlsShouldNotBeMerged()
    {
        new NonPublicUrlsShouldNotBeMerged().BDDfy<MergeSchemaTests>();
    }
        
    [Test]
    public void SecurityRequirementsShouldBeAddedAfterMerge()
    {
        new SecurityRequirementsShouldBeAddedAfterMerge().BDDfy<MergeSchemaTests>();
    }
        
    [Test]
    public void ApiShouldBeRenamedAfterMerge()
    {
        new ApiShouldBeRenamedAfterMerge().BDDfy<MergeSchemaTests>();
    }
}