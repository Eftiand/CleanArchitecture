using System.Reflection;
using coaches.Modules.Shared.Application.Common.Constants;
using coaches.Modules.Shared.Contracts.Constants;
using coaches.Modules.Shared.Contracts.Events.@base;
using coaches.Modules.Shared.Contracts.Modules;
using coaches.Modules.Shared.Domain.BaseEntities;
using NetArchTest.Rules;

namespace coaches.Infrastructure.IntegrationTests.ArchitectureTests;

public class ArchitectureTests
{
    private const string InfrastructureNamespace = CommonConstants.Assemblies.Infrastructure;
    private const string WebNamespace = CommonConstants.Assemblies.WebApi;

    [Test]
    public void Infrastructure_Should_Not_DependOnWeb()
    {
        var assembly = GetAssembly(AssemblyType.Infrastructure);
        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOn(WebNamespace)
            .GetResult();

        AssertResult(result);
    }

    [Test]
    public void Controllers_Should_Not_ReferenceInfrastructure()
    {
        var assembly = GetAssembly(AssemblyType.Web);

        var result = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Controller")
            .ShouldNot()
            .HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        AssertResult(result);
    }

    [Test]
    public void DomainEvents_Should_Inherit_From_BaseEvent()
    {
        var assembly = GetAssembly(AssemblyType.SharedDomain);

        var result = Types
            .InAssembly(assembly)
            .That()
            .HaveNameEndingWith("Event")
            .And()
            .AreNotAbstract()
            .And()
            .AreNotInterfaces()
            .Should()
            .Inherit(typeof(BaseEvent))
            .GetResult();
        AssertResult(result);
    }

    [Test]
    public void DomainProjects_Should_Not_Reference_ApplicationOrContracts()
    {
        var assembly = GetAssembly(AssemblyType.Domain);
        var forbidden = new[]
        {
            CommonConstants.Assemblies.Application,
            CommonConstants.Assemblies.Contracts
        };

        var result = Types
            .InAssembly(assembly)
            .ShouldNot()
            .HaveDependencyOnAny(forbidden)
            .GetResult();

        AssertResult(result);
    }

    private static IEnumerable<string> GetModules()
    {
        return AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IModuleRegistration).IsAssignableFrom(t) && !t.IsInterface)
            .Select(t => t.Namespace?.Split('.')[2])
            .Where(n => n != null)!;
    }

    private static void AssertResult(TestResult result)
    {
        if (!result.IsSuccessful)
        {
            var failingTypes = result.FailingTypeNames
                .OrderBy(x => x)
                .Select(name => $"- {name}");

            var errorMessage =
                $"Architecture test failed.\n" +
                $"Failing types:\n" +
                $"{string.Join("\n", failingTypes)}";

            Assert.Fail(errorMessage);
        }
    }
}
