using System.Reflection;
namespace Architecture.Tests;

public class ArchitectureTest
{
    private readonly static Type coreType = typeof(Ecommerce.Core.Entities.BaseEntity);
    private readonly static Type ApiType = typeof(Ecommerce.Api.Dtos.Basket.BasketProductDto);
    private readonly static Type InfrastructureType = typeof(Ecommerce.Infrastructure.Services.EmailService);

    private readonly Assembly coreAssembly = Assembly.GetAssembly(coreType)!;
    private readonly Assembly apiAssembly = Assembly.GetAssembly(ApiType)!;
    private readonly Assembly infrastructureAssembly = Assembly.GetAssembly(InfrastructureType)!;

    [Fact]
    public void Core_Should_NotHaveADependencyOnAnyLayer()
    {
        coreAssembly.Should().NotReference(infrastructureAssembly);
        coreAssembly.Should().NotReference(apiAssembly);
    }

    [Fact]
    public void Infrastructure_Should_OnlyDependsOnCoreLayer()
    {
        infrastructureAssembly.Should().Reference(coreAssembly);
        infrastructureAssembly.Should().NotReference(apiAssembly);
    }

    [Fact]
    public void Api_Should_DependsOnCoreAndInfrastructureLayers()
    {
        apiAssembly.Should().Reference(coreAssembly);
        apiAssembly.Should().Reference(infrastructureAssembly);
    }
}