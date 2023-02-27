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
using AutoMapper.QueryableExtensions;

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

    [HttpGet("GetAllStores")]
    [ProducesResponseType(typeof(IEnumerable<StoreResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StoreResponse>>> GetAllStores()
    {
        List<StoreResponse> stores = await _db.Stores.ProjectTo<StoreResponse>(_mapper.ConfigurationProvider).ToListAsync();

        return Ok(stores);
    }

    [HttpGet("GetStoreById/{id}", Name = "GetStoreById")]
    [ProducesResponseType(typeof(StoreResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StoreResponse>> GetStoreById(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Store? store = await _db.Stores.FirstOrDefaultAsync(x => x.Id == id);

        if (store is null) return NotFound($"Could not found the store with the Id::{id}");

        return Ok(_mapper.Map<StoreResponse>(store));
    }

    [HttpPost("CreateStore")]
    [ProducesResponseType(typeof(StoreResponse), StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateStore([FromBody] CreateStoreRequest storeDto)
    {
        Store store = _mapper.Map<Store>(storeDto);

        await _db.Stores.AddAsync(store);
        
        if (await _db.SaveChangesAsync() < 1) return BadRequest("Could not create the store");

        return RedirectToRoute(nameof(GetStoreById), new { id = store.Id });
    }

    [HttpPut("EditStore/{id}")]
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

    [HttpDelete("DeleteStore/{id}")]
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

    [HttpPost("IncreaseProductInStore")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> IncreaseProductInStore(int storeId, int productId)
    {
        if (storeId < 1 || productId < 1) return BadRequest("Invalid id");

        Result operationResult = await _storeService.IncreaseProductAsync(productId: productId, storeId: storeId);

        if (!operationResult.IsSuccess) return operationResult.ToActionResult();

        return Ok("Product increase successfully");
    }

    [HttpPost("DecreaseProductInStore")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> DecreaseProductInStore(int storeId, int productId)
    {
        if (storeId < 1 || productId < 1) return BadRequest("Invalid id");

        Result operationResult = await _storeService.DecreaseProductAsync(productId: productId, storeId: storeId);

        if (!operationResult.IsSuccess) return operationResult.ToActionResult();

        return Ok("Product decrease successfully");
    }

    [HttpGet("GetStoreWithProduct/{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(StoreWithProductResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<StoreWithProductResponse>> GetStoreWithProduct(int id)
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

        return Ok(new StoreWithProductResponse(_mapper.Map<StoreResponse>(store), storeProducts));
    }
}
