using Ecommerce.Core.Common;
using Ecommerce.Core.Entities;

namespace Ecommerce.Core.UnitTests.Entities;

public class BasketTest
{
    [Fact]
    public void IncreaseProductQuantity_With_Invalid_Value_Should_Throw_ArgumentException()
    {
        Basket basketMock = new();

        Action act = () => basketMock.IncreaseProductQuantity(0);

        act.Should().Throw<ArgumentException>().WithMessage("Amount to increase could not be less than 1");
    }

    [Fact]
    public void IncreaseProductQuantity_With_Valid_Value_Should_Increase_The_Quantity()
    {
        int amountToIncrease = 1;
        Product product = new("test", 100f, 1, 1, "https://test.com");
        Basket basketMock = new();
        basketMock.Product = product;

        basketMock.IncreaseProductQuantity(amountToIncrease);

        basketMock.Quantity.Should().Be(amountToIncrease);
    }

    [Fact]
    public void DecreaseProductQuantity_With_Invalid_Value_Should_Throw_ArgumentException()
    {
        Product product = new("test", 100f, 1, 1, "https://test.com");
        Basket basketMock = new();
        basketMock.Product = product;

        basketMock.IncreaseProductQuantity();

        Action act = () => basketMock.DecreaseProductQuantity(0);

        act.Should().Throw<ArgumentException>().WithMessage("Amount to decrease could not be less than 1");
    }

    [Fact]
    public void DecreaseProductQuantity_With_No_Quantity_Should_Throw_InvalidOperationException()
    {
        Basket basketMock = new();

        Action act = () => basketMock.DecreaseProductQuantity(1);

        act.Should().Throw<InvalidOperationException>().WithMessage("The Basket doesn't have a quantity to decrease");
    }
}