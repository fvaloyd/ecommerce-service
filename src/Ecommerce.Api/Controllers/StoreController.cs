using Ecommerce.Core.Enums;
using Ecommerce.Core.Entities;
using Ecommerce.Api.Dtos.Store;
using Ecommerce.Application.Data;
using Ecommerce.Application.Stores;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Api.Dtos.Product;
using Francisvac.Result;

namespace Ecommerce.Api.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class StoreController : ApiControllerBase
{
    private readonly IStoreService _storeService;
    private readonly IEcommerceDbContext _db;
    private readonly IMapper _mapper;

    public StoreController(
        IStoreService storeService,
        IMapper mapper,
        IEcommerceDbContext db)
    {
        _storeService = storeService;
        _mapper = mapper;
        _db = db;
    }

    [HttpGet("GetAll")]
    [ProducesResponseType(typeof(IEnumerable<Store>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Store>>> GetAllStores()
    {
        List<Store> stores = await _db.Stores.ToListAsync();

        return stores;
    }

    [HttpGet("GetById/{id}", Name = "GetStoreById")]
    [ProducesResponseType(typeof(Store), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Store>> GetStoreById(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Store? store = await _db.Stores.FirstOrDefaultAsync(x => x.Id == id);

        if (store is null) return NotFound($"Could not found the store with the Id::{id}");

        return store;
    }

    [HttpPost("Create")]
    [ProducesResponseType(typeof(Store), StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateStore([FromBody] CreateStoreRequest storeDto)
    {
        Store store = _mapper.Map<Store>(storeDto);

        await _db.Stores.AddAsync(store);
        
        if (await _db.SaveChangesAsync() < 1) return BadRequest("Could not create the store");

        return RedirectToRoute(nameof(GetStoreById), new { id = store.Id });
    }

    [HttpPut("Edit/{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EditStore(int id, EditStoreRequest storeDto)
    {
        if (id < 1) return BadRequest("Invalid id");

        Store? storeToUpdate = await _db.Stores.FirstOrDefaultAsync(s => s.Id == id);

        if (storeToUpdate is null) return NotFound($"Could not found the store with the Id::{id}");

        _mapper.Map(storeDto, storeToUpdate);

        _db.Stores.Update(storeToUpdate);

        if (await _db.SaveChangesAsync() < 1) return BadRequest("Could not update the store");

        return NoContent();
    }

    [HttpDelete("Delete/{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteStore(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Result operationResult = await _storeService.DeleteStore(id);

        if (!operationResult.IsSuccess) return operationResult.ToActionResult();

        return NoContent();
    }

    [HttpPost("IncreaseProduct")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> IncreaseProduct(int storeId, int productId)
    {
        if (storeId < 1 || productId < 1) return BadRequest("Invalid id");

        Result operationResult = await _storeService.IncreaseProductAsync(productId: productId, storeId: storeId);

        if (!operationResult.IsSuccess) return operationResult.ToActionResult();

        return Ok("Product increase successfully");
    }

    [HttpPost("DecreaseProduct")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> DecreaseProduct(int storeId, int productId)
    {
        if (storeId < 1 || productId < 1) return BadRequest("Invalid id");

        Result operationResult = await _storeService.DecreaseProductAsync(productId: productId, storeId: storeId);

        if (!operationResult.IsSuccess) return operationResult.ToActionResult();

        return Ok("Product decrease successfully");
    }

    [HttpGet("GetStoreWithProducts/{id}")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(StoreResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<StoreResponse>> GetStoreWithProductsAsync(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Store? store = await _db.Stores.FirstOrDefaultAsync(s => s.Id == id);

        if (store is null) return NotFound("Could not found the store");

        IEnumerable<ProductResponse> storeProducts = await _db.ProductStores
                                                    .Include(ps => ps.Product)
                                                    .ThenInclude(p => p.Brand)
                                                    .Include(ps => ps.Product)
                                                    .ThenInclude(p => p.Category)
                                                    .Where(s => s.StoreId == id)
                                                    .Select(ps => _mapper.Map<ProductResponse>(ps.Product))
                                                    .ToListAsync();

        if (storeProducts is null || !storeProducts.Any()) return NotFound("Could not found products in the store");

        return Ok(new StoreResponse(store, storeProducts));
    }
}
