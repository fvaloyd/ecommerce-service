using Ecommerce.Core.Entities;
using Ecommerce.Api.Dtos.Basket;
using Ecommerce.Api.Dtos.Product;
using Ecommerce.Application.Baskets;
using Ecommerce.Application.Common.Interfaces;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Francisvac.Result;

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

    [HttpPost("AddProductToBasket")]
    public async Task<IActionResult> AddProductToBasket(int productId)
        => await _basketService.AddProductAsync(productId, _currentUserService.UserId!).ToActionResult();

    [HttpPost("IncreaseProductQuantityInBasket")]
    public async Task<IActionResult> IncreaseProductQuantityInBasket(int productId)
        => await _basketService.IncreaseProduct(productId, _currentUserService.UserId!).ToActionResult();

    [HttpPost("DecreaseProductQuantityInBasket")]
    public async Task<IActionResult> DecreaseProductQuantityInBasket(int productId)
        => await _basketService.DecreaseProduct(productId, _currentUserService.UserId!).ToActionResult();

    [HttpDelete("RemoveProductFromBasket")]
    public async Task<IActionResult> RemoveProductFromBasket(int productId)
        => await _basketService.RemoveProduct(productId, _currentUserService.UserId!).ToActionResult();

    [HttpGet("GetAllProductInBasket")]
    public async Task<ActionResult<BasketResponse>> GetAllProduct()
    {
        var userId = _currentUserService.UserId;

        Result<(IEnumerable<Product>, float)> operationResult = await _basketService.GetAllProducts(userId!);

        if (!operationResult.IsSuccess) return operationResult.ToActionResult();

        BasketResponse basketResponse = new(Total: operationResult.Data.Item2, Products: operationResult.Data.Item1.Select(p => _mapper.Map<ProductResponse>(p)));

        return Ok(basketResponse);
    }
}
