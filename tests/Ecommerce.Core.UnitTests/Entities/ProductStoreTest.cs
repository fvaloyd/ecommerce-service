using Ecommerce.Core.Entities;

namespace Ecommerce.Core.UnitTests.Entities;

public class ProductStoreTest
{
    [Fact]
    public void IncreaseQuantity_ShouldIncreaseTheQuantity_WhenValidQuantityIsPassed()
    {
        // Arrange
        int amountToIncrease = 1;

        ProductStore productStoreMock = new();

        // Act
        productStoreMock.IncreaseQuantity(amountToIncrease);

        // Assert
        productStoreMock.Quantity.Should().Be(amountToIncrease);
    }

    [Fact]
    public void IncreaseQuantity_ShouldThrowArgumentException_WhenInvalidQuantityIsPassed()
    {
        // Arrange
        int amountToIncrease = 0;

        ProductStore productStoreMock = new();

        // Act
        Action act = () => productStoreMock.IncreaseQuantity(amountToIncrease);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Amount could not be less than 1");
    }

    [Fact]
    public void IncreaseQuantity_ShouldIncreaseTheQuantityWithDefaultQuantityValue()
    {
        // Arrange
        int defaultQuantityValue = 1;

        ProductStore productStoreMock = new();

        // Act
        productStoreMock.IncreaseQuantity();

        // Assert
        productStoreMock.Quantity.Should().Be(defaultQuantityValue);
    }

    [Fact]
    public void DecreaseQuantity_ShouldThrowArgumentException_WhenInvalidQuantityIsPassed()
    {
        // Arrange
        int amountToDecrease = 0;

        ProductStore productStoreMock = new();

        // Act
        Action act = () => productStoreMock.DecreaseQuantity(amountToDecrease);

        // Assert
        act.Should().Throw<ArgumentException>().WithMessage("Amount to decrease could not be less than 1");
    }

    [Fact]
    public void DecreaseQuantity_ShouldDecreaseTheWithDefaultQuantityValue()
    {
        // Arrange
        int totalQuantity = 2;
        int defaultQuantityValue = 1;
        
        ProductStore productStoreMock = new();
        
        productStoreMock.IncreaseQuantity(totalQuantity);
        
        // Act
        productStoreMock.DecreaseQuantity();

        // Assert
        productStoreMock.Quantity.Should().Be(totalQuantity - defaultQuantityValue);
    }

    [Fact]
    public void DecreaseQuantity_ShouldDecreaseTheQuantity_WhenValidQuantityIsPassed()
    {
        // Arrange
        int totalQuantity = 5;
        
        int amountToDecrease = 3;

        ProductStore productStoreMock = new();
        
        productStoreMock.IncreaseQuantity(totalQuantity);
        
        // Act
        productStoreMock.DecreaseQuantity(amountToDecrease);

        // Assert
        productStoreMock.Quantity.Should().Be(totalQuantity - amountToDecrease);
    }

    [Fact]
    public void DecreaseQuantity_ShouldThrowInvalidOperationException_WhenAmountToDeCreaseIsGreaterThanQuantity()
    {
        // Arrange
        int totalQuantity = 5;
        
        int amountToDecrease = 6;

        ProductStore productStoreMock = new();
        
        productStoreMock.IncreaseQuantity(totalQuantity);

        // Act
        Action act = () => productStoreMock.DecreaseQuantity(amountToDecrease);

        // Assert
        act.Should().Throw<InvalidOperationException>("Amount to decrease could not be greater than Quantity");
    }
}