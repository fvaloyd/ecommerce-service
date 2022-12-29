using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.UnitTests.Entities;

public class BrandTest
{
    [Fact]
    public void ShouldInheritBaseEntity()
    {
        typeof(Brand).Should().BeAssignableTo<BaseEntity>();
    }

    [Theory]
    [InlineData("Dell")]
    [InlineData("Hp")]
    [InlineData("Razer")]
    [InlineData("Predator")]
    public void SetName_ShouldSetTheName_WhenValidNameIsPassed(string validName)
    {
        // Arrange
        Brand brandMock = new();

        // Act
        brandMock.SetName(validName);

        // Assert
        brandMock.Name.Should().Be(validName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("h")]
    public void SetName_ShouldThrowArgumentException_WhenInvalidNameIsPassed(string invalidName)
    {
        // Arrange
        Brand brandMock = new();

        // Act
        Action act = () => brandMock.SetName(invalidName);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("The length of the name could not be less than 1");
    }

    [Fact]
    public void SetName_ShouldThrowArgumentNullException_WithNullNameIsPassed()
    {
        // Arrange
        Brand brandMock = new();

        // Act
        Action act = () => brandMock.SetName(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}