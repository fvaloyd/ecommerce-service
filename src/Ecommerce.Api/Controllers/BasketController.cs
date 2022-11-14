using AutoMapper;
using Ecommerce.Api.Dtos.Basket;
using Ecommerce.Api.Dtos.Product;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[Authorize]
public class BasketController : ApiControllerBase
{
    private readonly IEfRepository<Basket> _basketRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IBasketService _basketService;
    private readonly IMapper _mapper;

    public BasketController(
        IEfRepository<Basket> basketRepo,
        IBasketService basketService,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
        _basketRepo = basketRepo;
        _basketService = basketService;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    [HttpPost("AddProduct")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddProduct(int productId)
    {
        var userId = _currentUserService.UserId;

        bool operationResult = await _basketService.AddProductAsync(productId: productId, userId: userId!);

        if (operationResult is false) return BadRequest("Could not add the product to the basket");

        return Ok("Product added successfully");
    }

    [HttpPost("IncreaseProductQuantity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IncreaseProductQuantity(int productId)
    {
        var userId = _currentUserService.UserId;

        bool operationResult = await _basketService.IncreaseProduct(productId: productId, userId: userId!);

        if (operationResult is false) return BadRequest("Could not increase the quantity");

        return Ok("Quantity increase successfully");
    }

    [HttpPost("DecreaseProductQuantity")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DecreaseProductQuantity(int productId)
    {
        var userId = _currentUserService.UserId;

        bool operationResult = await _basketService.DecreaseProduct(productId: productId, userId: userId!);

        if (operationResult is false) return BadRequest("Could not decrease the quantity");

        return Ok("Quantity decrease successfully");
    }

    [HttpDelete("RemoveProduct")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveProduct(int productId)
    {
        var userId = _currentUserService.UserId;

        Basket productToDelete = _basketRepo.GetFirst(b => b.ProductId == productId && b.ApplicationUserId == userId, IncludeProperty: "Product");

        if (productToDelete is null) return BadRequest("Could not found the product in your basket");

        bool RestoreProductQuantityIntoStoreResult = await _basketService.RestoreTheQuantityIntoStore(productToDelete);

        if (RestoreProductQuantityIntoStoreResult is false) return BadRequest("Could not restore the product quantity to the store");

        _basketRepo.Remove(productToDelete);

        int operationResult = await _basketRepo.SaveChangeAsync();

        if (operationResult < 1) return BadRequest("Could not remove the product");

        return NoContent();
    }

    [HttpGet("GetAllProduct")]
    [ProducesResponseType(typeof(BasketProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<BasketProductDto> GetAllProduct()
    {
        var userId = _currentUserService.UserId;

        IEnumerable<Basket> userBasket = _basketRepo.GetAll(b => b.ApplicationUserId == userId, IncludeProperty: "Product"); // TODO: Modify repository to be able to include depth prop.

        if (userBasket.Count() == 0) return BadRequest("No basket associated");

        IEnumerable<Product> userBasketProducts = userBasket.Select(ub => ub.Product).ToList();

        if (userBasketProducts.Count() == 0) return BadRequest("No product in the basket");

        BasketProductDto basketProductDto = new BasketProductDto
        (
            Products: userBasketProducts.Select(ub => _mapper.Map<GetProductDto>(ub)).ToList(),
            Total: userBasket.Select(ub => ub.Total).ToList().Aggregate((acc, next) => acc + next)
        );

        return Ok(basketProductDto);
    }
}