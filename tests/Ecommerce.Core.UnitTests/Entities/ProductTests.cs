using Ecommerce.Core.Entities;

namespace Ecommerce.Core.UnitTests.Entities;

public class ProductTest
{
    [Fact]
    public void Should_Inherit_BaseEntity()
    {
        typeof(Product).Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void ChangePrice_With_Valid_Params_Should_Change_The_Price()
    {
        float newPrice = 199.99f;
        Product productMock = new("product1", 100, 1, 1, "http://facebook.com");

        productMock.ChangePrice(newPrice);

        productMock.Price.Should().Be(newPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ChangePrice_With_Invalid_Params_Should_Throw_ArgumentException(float invalidValue)
    {
        Product productMock = new("product1", 100, 1, 1, "https://Facebook.com");       

        Action act = () => productMock.ChangePrice(invalidValue);
        
        act.Should().Throw<ArgumentException>()
                    .WithMessage("The price could not be less than 1");
    }

    [Fact]
    public void SetName_With_Invalid_Name_Should_Throw_ArgumentException()
    {
        Product productMock = new();

        Action act = () => productMock.SetName("");

        act.Should().Throw<ArgumentException>()
                    .WithMessage("The Name could not have a length less than 1");
    }

    [Fact]
    public void SetName_With_Null_Name_Should_Throw_ArgumentNullException()
    {
        Product productMock = new();

        Action act = () => productMock.SetName(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("Laptop")]
    [InlineData("Phone")]
    [InlineData("Iphone 14 Pro Max Full HD 4K")]
    public void SetName_With_Valid_Name_Should_Set_The_Name(string validName)
    {
        Product productMock = new();

        productMock.SetName(validName);

        productMock.Name.Should().Be(validName);
    }
    
    [Theory]
    [InlineData("https://facebook.com/")]
    [InlineData("http://facebook.com/")]
    public void SetImage_With_Valid_Uri_Should_Set_The_Image(string validUrl)
    {
        Product productMock = new();

        productMock.SetImage(validUrl);

        productMock.ImageUrl.Should().Be(validUrl);
    }

    [Fact]
    public void SetImageUrl_With_Invalid_Uri_Should_Throw_ArgumentException()
    {
        string invalidUrl = "facebook.com";
        Product productMock = new();

        Action act = () => productMock.SetImage(invalidUrl);

        act.Should().Throw<ArgumentException>()
                    .WithMessage("The ImageUrl is invalid");
    }

    [Fact]
    public void SetImageUrl_With_Null_Uri_Should_Throw_ArgumentNullException()
    {
        Product productMock = new();

        Action act = () => productMock.SetImage(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}