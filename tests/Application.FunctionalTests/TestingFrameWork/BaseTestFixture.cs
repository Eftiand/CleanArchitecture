namespace CleanArchitecture.Application.FunctionalTests.TestingFrameWork;

[TestFixture]
public abstract class BaseTestFixture : Testing
{
    [SetUp]
    public async Task TestSetUp()
    {
        await ResetState();
   }
}
