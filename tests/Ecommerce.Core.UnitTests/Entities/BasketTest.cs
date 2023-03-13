using Ecommerce.Core.Entities;

namespace Ecommerce.Core.UnitTests.Entities;

public class BasketTest
{
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
    public void IncreaseProductQuantity_ShouldThrowArgumentOutOfRangeException_WhenInValidQuantityIsPassed()
    {
        // Arrange
        int invalidAmountToIncrease = 0;
        
        Basket basket = new(){Product = new("test", 100f, 1, 1, "https://test.com")};

        // Act
        Func<int> result = () => basket.IncreaseProductQuantity(invalidAmountToIncrease);

        // Assert
        result.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void DecreaseProductQuantity_ShouldReturnDecreasedQuantity_WhenInvalidQuantityIsPassed()
    {
        // Arrange
        int quantityToDecrease = 1;
        Basket basket = new() { Product = new("test", 100f, 1, 1, "https://test.com") };

        basket.IncreaseProductQuantity();

        // Assert
        var result = basket.DecreaseProductQuantity(quantityToDecrease);

        // Act
        result.Should().Be(quantityToDecrease);
        basket.Quantity.Should().Be(0);
    }

    [Fact]
    public void DecreaseProductQuantity_ShouldZero_WhenTheProductDoesnHaveQuantity()
    {
        // Arrange
        Basket basketWithNoProductQuantity = new();

        // Assert
        int result = basketWithNoProductQuantity.DecreaseProductQuantity(1);

        // Act
        result.Should().Be(0);
    }

    [Fact]
    public void DecreaseProductQuantity_ShouldThrowArgumentOutOfRangeException_WhenInvalidAmountToDecreaseIsSend()
    {
        // Arrange
        int invalidAmountToDecrease = 0;
        Basket basket = new();

        // Assert
        Func<int> result = () => basket.DecreaseProductQuantity(invalidAmountToDecrease);

        // Act
        result.Should().Throw<ArgumentOutOfRangeException>();
    }
}