using System.Reflection;
namespace Architecture.Tests;

public class ArchitectureTest
{
    readonly Assembly coreAssembly = Assembly.GetAssembly(typeof(Ecommerce.Core.AssemblyReference))!;
    readonly Assembly applicationAssembly = Assembly.GetAssembly(typeof(Ecommerce.Application.AssemblyReference))!;
    readonly Assembly apiAssembly = Assembly.GetAssembly(typeof(Ecommerce.Api.AssemblyReference))!;
    readonly Assembly infrastructureAssembly = Assembly.GetAssembly(typeof(Ecommerce.Infrastructure.AssemblyReference))!;

    [Fact]
    public void Core_ShouldNotHaveADependencyOnAnyLayer()
    {
        coreAssembly.Should().NotReference(infrastructureAssembly);
        coreAssembly.Should().NotReference(apiAssembly);
        coreAssembly.Should().NotReference(applicationAssembly);
    }
    
    [Fact]
    public void Application_ShouldOnlyDependsOnCoreLayer()
    {
        applicationAssembly.Should().Reference(coreAssembly);
        applicationAssembly.Should().NotReference(infrastructureAssembly);
        applicationAssembly.Should().NotReference(apiAssembly);
    }

    [Fact]
    public void Infrastructure_ShouldOnlyDependsOnApplicationLayer()
    {
        infrastructureAssembly.Should().Reference(coreAssembly);
        infrastructureAssembly.Should().Reference(applicationAssembly);
        infrastructureAssembly.Should().NotReference(apiAssembly);
    }

    [Fact]
    public void Api_ShouldDependsOnInfrastructureLayers()
    {
        apiAssembly.Should().Reference(coreAssembly);
        apiAssembly.Should().Reference(infrastructureAssembly);
        apiAssembly.Should().Reference(applicationAssembly);
    }
}