using Ecommerce.Core.Entities;
using Ecommerce.Application.Baskets;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Contracts.Baskets;
using Ecommerce.Contracts.Products;
using Ecommerce.Contracts.Endpoints;

using AutoMapper;
using Francisvac.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

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
    [Route(BasketEndpoints.AddProduct + "{productId}")]
    public async Task<IActionResult> AddProduct(int productId)
        => await _basketService.AddProductAsync(productId, _currentUserService.UserId!).ToActionResult();

    [HttpPost]
    [Route(BasketEndpoints.IncreaseProduct + "{productId}")]
    public async Task<IActionResult> IncreaseProduct(int productId)
        => await _basketService.IncreaseProduct(productId, _currentUserService.UserId!).ToActionResult();

    [HttpPost]
    [Route(BasketEndpoints.DecreaseProduct + "{productId}")]
    public async Task<IActionResult> DecreaseProduct(int productId)
        => await _basketService.DecreaseProduct(productId, _currentUserService.UserId!).ToActionResult();

    [HttpDelete]
    [Route(BasketEndpoints.RemoveProduct + "{productId}")]
    public async Task<IActionResult> RemoveProduct(int productId)
        => await _basketService.RemoveProduct(productId, _currentUserService.UserId!).ToActionResult();

    [HttpGet]
    [Route(BasketEndpoints.GetProducts)]
    public async Task<ActionResult<BasketResponse>> GetProducts()
    {
        var userId = _currentUserService.UserId;

        Result<(IEnumerable<Product>, float)> operationResult = await _basketService.GetAllProducts(userId!);

        if (!operationResult.IsSuccess) return operationResult.ToActionResult();

        BasketResponse basketResponse = new(Total: operationResult.Data.Item2, Products: operationResult.Data.Item1.Select(p => _mapper.Map<ProductResponse>(p)));

        return Ok(basketResponse);
    }
}
