using Ecommerce.Core.Entities;
using FluentAssertions;
using Xunit;

namespace Ecommerce.Core.UnitTests.Entities;

public class StoreTest
{
    [Fact]
    public void Should_Inherit_BaseEntity()
    {
        typeof(Store).Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void State_Could_Be_True_In_New_Instance()
    {
        Store storeMock = new();

        storeMock.State.Should().Be(true);
    }

    [Fact]
    public void ChangeState_Should_Switch_The_State()
    {
        Store storeMock = new();

        storeMock.ChangeState();

        storeMock.State.Should().Be(false);

        storeMock.ChangeState();

        storeMock.State.Should().Be(true);
    }

    [Fact]
    public void SetName_With_Invalid_Value_Should_Throw_ArgumentException()
    {
        Store storeMock = new();

        Action act = () => storeMock.SetName("");

        act.Should().Throw<ArgumentException>().WithMessage("Name length could not be less than 1");
    }

    [Fact]
    public void SetName_With_Null_Value_Should_Throw_ArgumentNullException()
    {
        Store storeMock = new();

        Action act = () => storeMock.SetName(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}