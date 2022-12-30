using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.UnitTests.Entities;

public class StoreTest
{
    [Fact]
    public void ShouldInheritBaseEntity()
    {
        typeof(Store).Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void TheState_CouldBeTrueInNewInstance()
    {
        // Arrange
        Store storeMock = new();

        // Assert
        storeMock.State.Should().Be(true);
    }

    [Fact]
    public void ChangeState_ShouldSwitchTheState()
    {
        // Arrange
        Store storeMock = new();

        // Act
        storeMock.ChangeState();

        // Assert
        storeMock.State.Should().Be(false);

        // Act
        storeMock.ChangeState();

        // Assert
        storeMock.State.Should().Be(true);
    }

    [Fact]
    public void SetName_ShouldThrowArgumentException_WhenInvalidNameIsPassed()
    {
        // Arrange
        Store storeMock = new();

        // Act
        Action act = () => storeMock.SetName("");

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Name length could not be less than 1");
    }

    [Fact]
    public void SetName_ShouldThrowArgumentNullException_WhenNullNameIsPassed()
    {
        // Arrange
        Store storeMock = new();

        // Act
        Action act = () => storeMock.SetName(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}