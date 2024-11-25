using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.FunctionalTests.TestingFrameWork;

[TestFixture]
public abstract class BaseTestFixture : Testing
{
    public IApplicationDbContext Session => DbContext;

    [SetUp]
    public async Task TestSetUp()
    {
        await ResetState();
    }
}
