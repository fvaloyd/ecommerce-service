using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.UnitTests.Entities;

public class BrandTest
{
    [Fact]
    public void Should_Inherit_BaseEntity()
    {
        typeof(Brand).Should().BeAssignableTo<BaseEntity>();
    }

    [Theory]
    [InlineData("Dell")]
    [InlineData("Hp")]
    [InlineData("Razer")]
    [InlineData("Predator")]
    public void SetName_With_Valid_Params_Should_Set_The_Name(string validName)
    {
        Brand brandMock = new();

        brandMock.SetName(validName);

        brandMock.Name.Should().Be(validName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("h")]
    public void SetName_With_Invalid_Name_Should_Throw_ArgumentException(string invalidName)
    {
        Brand brandMock = new();

        Action act = () => brandMock.SetName(invalidName);

        act.Should().Throw<ArgumentException>().WithMessage("The length of the name could not be less than 1");
    }

    [Fact]
    public void SetName_With_Null_Name_Should_Throw_ArgumentNullException()
    {
        Brand brandMock = new();

        Action act = () => brandMock.SetName(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}