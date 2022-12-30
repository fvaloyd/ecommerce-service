using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.UnitTests.Entities;

public class ProductTest
{
    [Fact]
    public void ShouldInheritBaseEntity()
    {
        typeof(Product).Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void ChangePrice_ShouldChangeThePrice_WhenValidPriceIsPassed()
    {
        // Arrange
        float validPrice = 199.99f;
        
        Product productMock = new("product1", 100, 1, 1, "http://facebook.com");

        // Act
        productMock.ChangePrice(validPrice);

        // Assert
        productMock.Price.Should().Be(validPrice);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ChangePrice_ShouldThrowArgumentException_WhenInvalidPriceIsPassed(float invalidValue)
    {
        // Arrange
        Product productMock = new("product1", 100, 1, 1, "https://Facebook.com");       

        // Act
        Action act = () => productMock.ChangePrice(invalidValue);
        
        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("The price could not be less than 1");
    }

    [Fact]
    public void SetName_ShouldThrowArgumentException_WhenInvalidNameIsPassed()
    {
        // Arrange
        Product productMock = new();

        // Act
        Action act = () => productMock.SetName("");

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("The Name could not have a length less than 1");
    }

    [Fact]
    public void SetName_ShouldThrowArgumentNullException_WhenNullNameIsPassed()
    {
        // Arrange
        Product productMock = new();

        // Act
        Action act = () => productMock.SetName(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("Laptop")]
    [InlineData("Phone")]
    [InlineData("Iphone 14 Pro Max Full HD 4K")]
    public void SetName_ShouldSetTheName_WhenValidNameIsPassed(string validName)
    {
        // Arrange
        Product productMock = new();

        // Act
        productMock.SetName(validName);

        // Assert
        productMock.Name.Should().Be(validName);
    }
    
    [Theory]
    [InlineData("https://facebook.com/")]
    [InlineData("http://facebook.com/")]
    public void SetImage_ShouldSetTheImage_WhenValidUriIsPassed(string validUrl)
    {
        // Arrange
        Product productMock = new();

        // Act
        productMock.SetImage(validUrl);

        // Assert
        productMock.ImageUrl.Should().Be(validUrl);
    }

    [Fact]
    public void SetImageUrl_ShouldThrowArgumentException_WhenInvalidUriIsPassed()
    {
        // Arrange
        string invalidUrl = "facebook.com";
        
        Product productMock = new();

        // Act
        Action act = () => productMock.SetImage(invalidUrl);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("The ImageUrl is invalid");
    }

    [Fact]
    public void SetImageUrl_ShouldThrowArgumentNullException_WhenNullUriIsPassed()
    {
        // Arrange
        Product productMock = new();

        // Act
        Action act = () => productMock.SetImage(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}