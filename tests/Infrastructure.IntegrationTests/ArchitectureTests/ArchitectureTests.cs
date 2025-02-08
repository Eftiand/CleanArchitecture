using System.Reflection;
using coaches.Modules.Shared.Contracts.Constants;
using coaches.Modules.Shared.Contracts.Consumers;
using coaches.Modules.Shared.Domain.BaseEntities;
using coaches.Modules.Shared.Domain.BaseEntities.Base;
using Microsoft.EntityFrameworkCore;
using NetArchTest.Rules;

namespace coaches.Infrastructure.IntegrationTests.ArchitectureTests;

public class ArchitectureTests
{
    private const string InfrastructureNamespace = CommonConstants.Assemblies.Infrastructure;
    private const string WebNamespace = CommonConstants.Assemblies.WebApi;

    [Test]
    public void Infrastructure_Should_Not_DependOnWeb()
    {
        var assembly = Assembly.Load(InfrastructureNamespace);
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
        var assembly = Assembly.Load(WebNamespace);

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
    public void Consumers_Should_Inherit_From_BaseConsumer()
    {
        var modules = GetApplicationModules();

        var result = Types
            .InAssemblies(modules)
            .That()
            .HaveNameEndingWith("Consumer")
            .And()
            .AreNotAbstract()
            .Should()
            .Inherit(typeof(BaseConsumer<>))
            .GetResult();

        AssertResult(result);
    }


    [Test]
    public void Commands_Should_Inherit_From_BaseCommand()
    {
        var modules = GetContractModules();

        var result = Types
            .InAssemblies(modules)
            .That()
            .HaveNameEndingWith("Command")
            .And()
            .AreNotAbstract()
            .Should()
            .Inherit(typeof(BaseCommand<>))
            .GetResult();

        AssertResult(result);
    }

    [Test]
    public void Events_Should_Inherit_From_BaseEvent()
    {
        var modules = GetDomainModules();

        var result = Types
            .InAssemblies(modules)
            .That()
            .HaveNameEndingWith("Event")
            .And()
            .AreNotAbstract()
            .Should()
            .Inherit(typeof(BaseEvent))
            .GetResult();

        AssertResult(result);
    }

    [Test]
    public void ApplicationProjects_Should_Not_Reference_Other_ApplicationProjects()
    {
        var modules = GetApplicationModules()
            .Where(x => !x.FullName!.Contains("Shared"))
            .ToList();

        foreach (var module in modules)
        {
            var otherModuleNamespaces = modules
                .Where(m => m != module)
                .Select(m => m.GetName().Name!)
                .ToArray();

            var result = Types
                .InAssembly(module)
                .ShouldNot()
                .HaveDependencyOnAny(otherModuleNamespaces)
                .GetResult();

            AssertResult(result);
        }
    }


    [Test]
    public void ApplicationProjects_Should_Not_Reference_Others_DomainProjects()
    {
        var applicationModules = GetApplicationModules();
        var domainModules = GetDomainModules()
            .Where(x => !x.FullName!.Contains("Shared"))
            .ToList();

        foreach (var appModule in applicationModules)
        {
            var currentModuleName = GetModuleName(appModule.GetName().Name!); // e.g., "Todos"

            var otherDomainModules = domainModules
                .Where(d => GetModuleName(d.GetName().Name!) != currentModuleName)
                .Select(m => m.GetName().Name!)
                .ToArray();

            var result = Types
                .InAssembly(appModule)
                .ShouldNot()
                .HaveDependencyOnAny(otherDomainModules)
                .GetResult();

            AssertResult(result);
        }
    }

    [Test]
    public void DomainProjects_Should_Not_Reference_Others_DomainProjects()
    {
        var domainModules = GetDomainModules()
            .Where(x => !x.FullName!.Contains("Shared"))
            .ToList();

        foreach (var domainModule in domainModules)
        {
            var currentModuleName = GetModuleName(domainModule.GetName().Name!);

            var otherDomainModules = domainModules
                .Where(d => GetModuleName(d.GetName().Name!) != currentModuleName)
                .Select(m => m.GetName().Name!)
                .ToArray();

            var result = Types
                .InAssembly(domainModule)
                .ShouldNot()
                .HaveDependencyOnAny(otherDomainModules)
                .GetResult();

            AssertResult(result);
        }
    }

    [Test]
    public void DomainProjects_Should_Not_Reference_ApplicationProjects()
    {
        var domainModules = GetDomainModules()
            .ToList();

        var applicationModules = GetApplicationModules()
            .ToList();

        foreach (var domainModule in domainModules)
        {
            var result = Types
                .InAssembly(domainModule)
                .ShouldNot()
                .HaveDependencyOnAny(applicationModules.Select(m => m.GetName().Name!).ToArray())
                .GetResult();

            AssertResult(result);
        }
    }

    [Test]
    public void DomainProjects_Should_Not_Reference_InfrastructureProjects()
    {
        var domainModules = GetDomainModules();
        var infrastructureModule = Assembly.Load(InfrastructureNamespace);

        foreach (var domainModule in domainModules)
        {
            var result = Types
                .InAssembly(domainModule)
                .ShouldNot()
                .HaveDependencyOn(infrastructureModule.GetName().Name!)
                .GetResult();

            AssertResult(result);
        }
    }

    [Test]
    public void ApplicationProjects_Should_Not_Reference_InfrastructureProjects()
    {
        var applicationModules = GetApplicationModules();
        var infrastructureModule = Assembly.Load(InfrastructureNamespace);

        foreach (var appModule in applicationModules)
        {
            var result = Types
                .InAssembly(appModule)
                .ShouldNot()
                .HaveDependencyOn(infrastructureModule.GetName().Name!)
                .GetResult();

            AssertResult(result);
        }
    }

    [Test]
    public void InfrastructureProjects_Should_Not_Reference_ApplicationProjects()
    {
        var infrastructureModule = Assembly.Load(InfrastructureNamespace);
        var applicationModules = GetApplicationModules()
            .Select(m => m.GetName().Name!)
            .ToArray();

        var result = Types
            .InAssembly(infrastructureModule)
            .ShouldNot()
            .HaveDependencyOnAny(applicationModules)
            .GetResult();

        AssertResult(result);
    }

    [Test]
    public void Sagas_Should_Be_In_Application_Layer()
    {
        var applicationModules = GetApplicationModules();
        var otherModules = GetDomainModules()
            .Concat(GetContractModules())
            .Select(m => m.GetName().Name!)
            .ToArray();

        var result = Types
            .InAssemblies(applicationModules)
            .That()
            .HaveNameEndingWith("Saga")
            .Or()
            .HaveNameEndingWith("StateMachine")
            .Should()
            .ResideInNamespaceEndingWith("Application")
            .And()
            .NotHaveDependencyOnAny(otherModules)
            .GetResult();

        AssertResult(result);
    }

    [Test]
    public void Commands_Should_Be_In_Contracts_Project()
    {
        var allAssemblies = GetAllModules()
            .Where(x => !x.FullName!.Contains("Shared"))
            .ToList();

        var result = Types
            .InAssemblies(allAssemblies)
            .That()
            .HaveNameEndingWith("Command")
            .Should()
            .ResideInNamespaceContaining(".Contracts")
            .GetResult();

        AssertResult(result);
    }

    [Test]
    public void Entities_Should_Be_In_Domain_Project()
    {
        var allAssemblies = GetAllModules();

        var result = Types
            .InAssemblies(allAssemblies)
            .That()
            .Inherit(typeof(BaseEntity))
            .Should()
            .ResideInNamespaceContaining(".Domain")
            .GetResult();

        AssertResult(result);
    }

    [Test]
    public void Dtos_Should_Be_In_Contracts_Project()
    {
        var modules = GetAllModules();
        var result = Types
            .InAssemblies(modules)
            .That()
            .HaveNameEndingWith("Dto")
            .Or()
            .HaveNameEndingWith("Response")
            .Or()
            .HaveNameEndingWith("Request")
            .Should()
            .ResideInNamespaceEndingWith(".Contracts")
            .GetResult();

        AssertResult(result);
    }

    [Test]
    public void ValueObjects_Should_Be_In_Domain_Project()
    {
        var modules = GetAllModules();
        var result = Types
            .InAssemblies(modules)
            .That()
            .Inherit(typeof(ValueObject))
            .Should()
            .ResideInNamespaceEndingWith(".Domain")
            .GetResult();

        AssertResult(result);
    }

    [Test]
    public void Configurations_Should_Be_In_Infrastructure_Project()
    {
        var modules = GetAllModules();
        var result = Types
            .InAssemblies(modules)
            .That()
            .HaveNameEndingWith("Configuration")
            .And()
            .ImplementInterface(typeof(IEntityTypeConfiguration<>))
            .Should()
            .ResideInNamespaceContaining("Infrastructure")
            .GetResult();

        AssertResult(result);
    }

    [Test]
    public void ExceptionsShouldBeInCorrectProject()
    {
        var modules = GetAllModules();
        var result = Types
            .InAssemblies(modules)
            .That()
            .HaveNameEndingWith("Exception")
            .Should()
            .ResideInNamespaceEndingWith(".Domain")
            .Or()
            .ResideInNamespaceEndingWith(".Application")
            .GetResult();

        AssertResult(result);
    }

    // Gets module name like "Todos" from "coaches.Modules.Todos.Domain"
    private string GetModuleName(string assemblyName)
    {
        return assemblyName.Split(".")[2];
    }

    private static IEnumerable<Assembly> GetAllModules()
    {
        return GetDomainModules()
            .Concat(GetApplicationModules())
            .Concat(GetContractModules())
            .Concat([
                Assembly.Load(InfrastructureNamespace)
            ]);
    }

    private static IEnumerable<Assembly> GetApplicationModules()
    {
        return CommonConstants.Assemblies.Modules.AllApplications.Values
            .Select(Assembly.Load);
    }

    private static IEnumerable<Assembly> GetContractModules()
    {
        return CommonConstants.Assemblies.Modules.AllContracts.Values
            .Select(Assembly.Load);
    }

    private static IEnumerable<Assembly> GetDomainModules()
    {
        return CommonConstants.Assemblies.Modules.AllDomains.Values
            .Select(Assembly.Load);
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
