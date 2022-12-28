using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.UnitTests.Entities;

public class ProductStoreTest
{
    [Fact]
    public void IncreaseQuantity_With_Valid_Value_Should_Increase_The_Quantity()
    {
        int amountToIncrease = 1;

        ProductStore productStoreMock = new();

        productStoreMock.IncreaseQuantity(amountToIncrease);

        productStoreMock.Quantity.Should().Be(amountToIncrease);
    }

    [Fact]
    public void IncreaseQuantity_With_Invalid_Value_Should_Throw_ArgumentException()
    {
        int amountToIncrease = 0;

        ProductStore productStoreMock = new();

        Action act = () => productStoreMock.IncreaseQuantity(amountToIncrease);

        act.Should().Throw<ArgumentException>().WithMessage("Amount could not be less than 1");
    }

    [Fact]
    public void IncreaseQuantity_With_No_Params_Should_Increase_The_Quantity_By_One()
    {
        ProductStore productStoreMock = new();

        productStoreMock.IncreaseQuantity();

        productStoreMock.Quantity.Should().Be(1);
    }

    [Fact]
    public void DecreaseQuantity_With_Invalid_Value_Should_Throw_ArgumentException()
    {
        int amountToDecrease = 0;

        ProductStore productStoreMock = new();

        Action act = () => productStoreMock.DecreaseQuantity(amountToDecrease);

        act.Should().Throw<ArgumentException>().WithMessage("Amount to decrease could not be less than 1");
    }

    [Fact]
    public void DecreaseQuantity_With_No_Params_Should_Decrease_The_Quantity_By_One()
    {
        int totalQuantity = 2;
        ProductStore productStoreMock = new();
        productStoreMock.IncreaseQuantity(totalQuantity);
        productStoreMock.DecreaseQuantity();

        productStoreMock.Quantity.Should().Be(totalQuantity - 1);
    }

    [Fact]
    public void DecreaseQuantity_With_Valid_Value_Should_Decrease_The_Quantity()
    {
        int totalQuantity = 5;
        int amountToDecrease = 3;

        ProductStore productStoreMock = new();
        productStoreMock.IncreaseQuantity(totalQuantity);
        productStoreMock.DecreaseQuantity(amountToDecrease);

        productStoreMock.Quantity.Should().Be(totalQuantity - amountToDecrease);
    }

    [Fact]
    public void DecreaseQuantity_With_AmountToDecrease_Greater_Than_Quantity_Should_Throw_InvalidOperationException()
    {
        int totalQuantity = 5;
        int amountToDecrease = 6;

        ProductStore productStoreMock = new();
        productStoreMock.IncreaseQuantity(totalQuantity);

        Action act = () => productStoreMock.DecreaseQuantity(amountToDecrease);

        act.Should().Throw<InvalidOperationException>("Amount to decrease could not be greater than Quantity");
    }
}