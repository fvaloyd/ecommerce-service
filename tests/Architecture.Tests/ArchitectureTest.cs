using System.Reflection;
namespace Architecture.Tests;

public class ArchitectureTest
{
    private readonly static Type coreType = typeof(Ecommerce.Core.AssemblyReference);
    private readonly static Type ApiType = typeof(Ecommerce.Api.AssemblyReference);
    private readonly static Type ApplicationType = typeof(Ecommerce.Application.AssemblyReference);
    private readonly static Type InfrastructureType = typeof(Ecommerce.Infrastructure.AssemblyReference);

    private readonly Assembly coreAssembly = Assembly.GetAssembly(coreType)!;
    private readonly Assembly applicationAssembly = Assembly.GetAssembly(ApplicationType)!;
    private readonly Assembly apiAssembly = Assembly.GetAssembly(ApiType)!;
    private readonly Assembly infrastructureAssembly = Assembly.GetAssembly(InfrastructureType)!;

    [Fact]
    public void Core_Should_NotHaveADependencyOnAnyLayer()
    {
        coreAssembly.Should().NotReference(infrastructureAssembly);
        coreAssembly.Should().NotReference(apiAssembly);
        coreAssembly.Should().NotReference(applicationAssembly);
    }
    
    [Fact]
    public void Application_Should_OnlyDependesOnCoreLayer()
    {
        applicationAssembly.Should().Reference(coreAssembly);
        applicationAssembly.Should().NotReference(infrastructureAssembly);
        applicationAssembly.Should().NotReference(apiAssembly);
    }

    [Fact]
    public void Infrastructure_Should_OnlyDependsOnApplicationLayer()
    {
        infrastructureAssembly.Should().Reference(coreAssembly);
        infrastructureAssembly.Should().Reference(applicationAssembly);
        infrastructureAssembly.Should().NotReference(apiAssembly);
    }

    [Fact]
    public void Api_Should_DependsOnfrastructureLayers()
    {
        apiAssembly.Should().Reference(coreAssembly);
        apiAssembly.Should().Reference(infrastructureAssembly);
        apiAssembly.Should().Reference(applicationAssembly);
    }
}