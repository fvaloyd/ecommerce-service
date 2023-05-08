using Ecommerce.Contracts;
using Ecommerce.Core.Entities;
using Ecommerce.Contracts.Responses;
using Ecommerce.Application.Baskets;
using Ecommerce.Application.Common.Interfaces;

using AutoMapper;
using Francisvac.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Francisvac.Result.AspNetCore;

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

    [HttpPost]
    [Route(ApiRoutes.Basket.AddProduct)]
    public async Task<IActionResult> AddProduct([FromQuery] int productId)
        => await _basketService.AddProductAsync(productId, _currentUserService.UserId!).ToActionResult();

    [HttpPost]
    [Route(ApiRoutes.Basket.IncreaseProduct)]
    public async Task<IActionResult> IncreaseProduct([FromQuery] int productId)
        => await _basketService.IncreaseProduct(productId, _currentUserService.UserId!).ToActionResult();

    [HttpPost]
    [Route(ApiRoutes.Basket.DecreaseProduct)]
    public async Task<IActionResult> DecreaseProduct([FromQuery] int productId)
        => await _basketService.DecreaseProduct(productId, _currentUserService.UserId!).ToActionResult();

    [HttpDelete]
    [Route(ApiRoutes.Basket.RemoveProduct)]
    public async Task<IActionResult> RemoveProduct([FromQuery] int productId)
        => await _basketService.RemoveProduct(productId, _currentUserService.UserId!).ToActionResult();

    [HttpGet]
    [Route(ApiRoutes.Basket.GetProducts)]
    public async Task<ActionResult<BasketResponse>> GetProducts()
    {
        var userId = _currentUserService.UserId;

        Result<(IEnumerable<Product>, float)> operationResult = await _basketService.GetAllProducts(userId!);

        if (!operationResult.IsSuccess) return operationResult.ToActionResult();

        BasketResponse basketResponse = new(Total: operationResult.Data.Item2, Products: operationResult.Data.Item1.Select(p => _mapper.Map<ProductResponse>(p)));

        return Ok(basketResponse);
    }

    [HttpGet]
    [Route(ApiRoutes.Basket.GetProductIds)]
    public async Task<IActionResult> GetProductIds()
        => await  _basketService.GetProductIds(_currentUserService.UserId!).ToActionResult();
}
