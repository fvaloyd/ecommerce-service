using AutoMapper;
using Ecommerce.Api.Dtos.Basket;
using Ecommerce.Api.Dtos.Product;
using Ecommerce.Application.Baskets;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[Authorize]
public class BasketController : ApiControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IBasketService _basketService;
    private readonly IMapper _mapper;

    public BasketController(
        IBasketService basketService,
        IMapper mapper,
        ICurrentUserService currentUserService)
    {
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

        var operationResult = await _basketService.RemoveProduct(productId, userId!);

        if (operationResult == false) return BadRequest("Could not remove the product");

        return NoContent();
    }

    [HttpGet("GetAllProduct")]
    [ProducesResponseType(typeof(BasketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BasketResponse>> GetAllProduct()
    {
        var userId = _currentUserService.UserId;

        try
        {
            (IEnumerable<Product> basketProducts, float total) = await _basketService.GetAllProducts(userId!);
            
            BasketResponse basketResponse = new(Total: total, Products: basketProducts.Select(p => _mapper.Map<GetProductDto>(p)));
            
            return Ok(basketResponse);
        }
        catch (Exception)
        {
            throw;
        }
    }
}