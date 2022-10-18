using System.Security.Claims;
using AutoMapper;
using Ecommerce.Api.Dtos.Basket;
using Ecommerce.Api.Dtos.Product;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[Authorize]
public class BasketController : ApiControllerBase
{
    private readonly IEfRepository<Basket> _basketRepo;
    private readonly IBasketService _basketService;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public BasketController(
        IEfRepository<Basket> basketRepo,
        IBasketService basketService,
        IMapper mapper,
        UserManager<ApplicationUser> userManager)
    {
        _basketRepo = basketRepo;
        _basketService = basketService;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpPost("AddProduct")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProduct(int productId)
    {
        ClaimsPrincipal currentUser = HttpContext.User;
        string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        bool operationResult = await _basketService.AddProductAsync(productId: productId, userId: userId);

        if (operationResult is false) return BadRequest("Could not add the product to the basket");

        return Ok("Product added successfully");
    }

    [HttpPost("IncreaseProductQuantity")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IncreaseProductQuantity(int productId)
    {
        ClaimsPrincipal currentUser = HttpContext.User;
        string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        bool operationResult = await _basketService.IncreaseProduct(productId: productId, userId: userId);

        if (operationResult is false) return BadRequest("Could not increase the quantity");

        return Ok("Quantity increase successfully");
    }

    [HttpPost("DecreaseProductQuantity")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DecreaseProductQuantity(int productId)
    {
        ClaimsPrincipal currentUser = HttpContext.User;
        string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        bool operationResult = await _basketService.DecreaseProduct(productId: productId, userId: userId);

        if (operationResult is false) return BadRequest("Could not decrease the quantity");

        return Ok("Quantity decrease sucessfully");
    }

    [HttpDelete("RemoveProduct")]
    public async Task<IActionResult> RemoveProduct(int productId)
    {
        ClaimsPrincipal currentUser = HttpContext.User;
        string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        Basket productToDelete = _basketRepo.GetFirst(b => b.ProductId == productId && b.ApplicationUserId == userId, IncludeProperty: "Product");

        if (productToDelete is null) return NotFound("Could not found the product in your basket");

        bool RestoreProductQuantityIntoStoreResult = await _basketService.RestoreTheQuantityIntoStore(productToDelete);

        if (RestoreProductQuantityIntoStoreResult is false) return BadRequest("Could not restore the product quantity to the store");

        _basketRepo.Remove(productToDelete);

        int operationResult = await _basketRepo.SaveChangeAsync();

        if (operationResult < 1) return BadRequest("Could not remove the product");

        return NoContent();
    }

    [HttpGet("GetAllProduct")]
    public async Task<ActionResult<BasketProductDto>> GetAllProduct()
    {
        ClaimsPrincipal currentUser = HttpContext.User;
        string userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        IEnumerable<Basket> userBasket = await _basketRepo.GetAllAsync(b => b.ApplicationUserId == userId, IncludeProperty: "Product");

        if (userBasket.Count() == 0) return NotFound("No basket associated");

        IEnumerable<Product> userBasketProducts = userBasket.Select(ub => ub.Product).ToList();

        if (userBasketProducts.Count() == 0) return BadRequest("No product in the basket");

        BasketProductDto basketProductDto = new BasketProductDto
        (
            Products: userBasketProducts.Select(ub => _mapper.Map<GetProductDto>(ub)).ToList(),
            Total: userBasket.Select(ub => ub.Total).ToList().Aggregate((acc, next) => acc + next)
        );

        return basketProductDto;
    }
}