using Ecommerce.Core.Entities;

namespace Ecommerce.Core.UnitTests.Entities;

public class CategoryTest
{
    [Fact]
    public void Should_Inherit_BaseEntity()
    {
        typeof(Category).Should().BeAssignableTo<BaseEntity>();
    }

    [Theory]
    [InlineData("Phone")]
    [InlineData("Laptops")]
    public void SetName_With_Valid_Name_Should_Set_The_Name(string validName)
    {
        Category categoryMock = new();

        categoryMock.SetName(validName);

        categoryMock.Name.Should().Be(validName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("h")]
    public void SetName_With_Invalid_Name_Should_Throw_ArgumentException(string invalidName)
    {
        Category categoryMock = new();

        Action act = () => categoryMock.SetName(invalidName);

        act.Should().Throw<ArgumentException>().WithMessage("The length of the name could not be less than 1");
    }

    [Fact]
    public void SetName_With_Null_Name_Should_Throw_ArgumentNullException()
    {
        Category categoryMock = new();

        Action act = () => categoryMock.SetName(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}