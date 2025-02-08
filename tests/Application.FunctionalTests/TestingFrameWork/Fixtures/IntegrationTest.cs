using System.Threading.Tasks.Sources;
using coaches.Modules.Shared.Application.Common.Interfaces;
using coaches.Modules.Shared.Contracts.Common.Interfaces;

namespace coaches.Application.FunctionalTests.TestingFrameWork.Fixtures;

[TestFixture]
public abstract class IntegrationTest : IntegrationBaseFunctionality
{
    public IApplicationDbContext Session => DbContext;

}
