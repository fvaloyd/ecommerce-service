using Ecommerce.Core.Entities;

namespace Ecommerce.Core.UnitTests.Entities;

public class BasketTest
{
    [Fact]
    public void IncreaseProductQuantity_ShouldThrowArgumentException_WhenInvalidQuanityIsPassed()
    {
        // Arrange
        Basket basket = new();

        // Act
        Action act = () => basket.IncreaseProductQuantity(0);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Amount to increase could not be less than 1");
    }

    [Fact]
    public void IncreaseProductQuantity_ShouldIncreaseTheQuantity_WhenValidQuantityIsPassed()
    {
        // Arrange
        int amountToIncrease = 1;
        
        Basket basket = new(){Product = new("test", 100f, 1, 1, "https://test.com")};

        // Act
        basket.IncreaseProductQuantity(amountToIncrease);

        // Assert
        basket.Quantity.Should().Be(amountToIncrease);
    }

    [Fact]
    public void DecreaseProductQuantity_ShouldThrowArgumentException_WhenInvalidQuantityIsPassed()
    {
        // Arrange
        Basket basket = new() { Product = new("test", 100f, 1, 1, "https://test.com") };

        basket.IncreaseProductQuantity();

        // Act
        Action act = () => basket.DecreaseProductQuantity(0);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Amount to decrease could not be less than 1");
    }

    [Fact]
    public void DecreaseProductQuantity_ShouldThrowInvalidOperationException_WhenTheProductDoesnHaveQuantity()
    {
        // Arrange
        Basket basket = new();

        // Assert
        Action act = () => basket.DecreaseProductQuantity(1);

        // Act
        act.Should().Throw<InvalidOperationException>().WithMessage("The Basket doesn't have a quantity to decrease");
    }
}