using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.UnitTests.Entities;

public class CategoryTest
{
    [Fact]
    public void ShouldInheritBaseEntity()
    {
        typeof(Category).Should().BeAssignableTo<BaseEntity>();
    }

    [Theory]
    [InlineData("Phone")]
    [InlineData("Laptops")]
    public void SetName_ShouldSetTheName_WhenValidNameIsPassed(string validName)
    {
        // Arrange
        Category categoryMock = new();

        // Act
        categoryMock.SetName(validName);

        // Assert
        categoryMock.Name.Should().Be(validName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("h")]
    public void SetName_ShouldThrowArgumentException_WhenInvalidNameIsPassed(string invalidName)
    {
        // Arrange
        Category categoryMock = new();

        // Act
        Action act = () => categoryMock.SetName(invalidName);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("The length of the name could not be less than 1");
    }

    [Fact]
    public void SetName_ShouldThrowArgumentNullException_WhenNullNameIsPassed()
    {
        // Arrange
        Category categoryMock = new();

        // Act
        Action act = () => categoryMock.SetName(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}