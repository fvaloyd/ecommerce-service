using Ecommerce.Contracts;
using Ecommerce.Core.Enums;
using Ecommerce.Core.Entities;
using Ecommerce.Application.Data;
using Ecommerce.Contracts.Requests;
using Ecommerce.Application.Stores;
using Ecommerce.Contracts.Responses;
using Ecommerce.Application.Common.Models;

using AutoMapper;
using Francisvac.Result;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;


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

    [HttpGet]
    [Route(ApiRoutes.Store.GetAll)]
    [ProducesResponseType(typeof(IEnumerable<StoreResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<StoreResponse>>> Get()
    {
        List<StoreResponse> stores = await _db.Stores.ProjectTo<StoreResponse>(_mapper.ConfigurationProvider).ToListAsync();

        return Ok(stores);
    }

    [HttpGet]
    [Route(ApiRoutes.Store.GetById)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(StoreResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<StoreResponse>> Get(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Store? store = await _db.Stores.FirstOrDefaultAsync(x => x.Id == id);

        if (store is null) return NotFound($"Could not found the store with the Id::{id}");

        return Ok(_mapper.Map<StoreResponse>(store));
    }

    [HttpPost]
    [Route(ApiRoutes.Store.Create)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(StoreResponse), StatusCodes.Status302Found)]
    public async Task<IActionResult> Create([FromBody] CreateStoreRequest storeDto)
    {
        Store store = _mapper.Map<Store>(storeDto);

        await _db.Stores.AddAsync(store);
        
        if (await _db.SaveChangesAsync() < 1) return BadRequest("Could not create the store");

        return RedirectToAction("Get", new { id = store.Id });
    }

    [HttpPut]
    [Route(ApiRoutes.Store.Edit)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Edit(int id, EditStoreRequest storeDto)
    {
        if (id < 1) return BadRequest("Invalid id");

        Store? storeToUpdate = await _db.Stores.FirstOrDefaultAsync(s => s.Id == id);

        if (storeToUpdate is null) return NotFound($"Could not found the store with the Id::{id}");

        _mapper.Map(storeDto, storeToUpdate);

        _db.Stores.Update(storeToUpdate);

        if (await _db.SaveChangesAsync() < 1) return BadRequest("Could not update the store");

        return NoContent();
    }

    [HttpDelete]
    [Route(ApiRoutes.Store.Delete)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Result operationResult = await _storeService.DeleteStore(id);

        if (!operationResult.IsSuccess) return operationResult.ToActionResult();

        return NoContent();
    }

    [HttpPost]
    [Route(ApiRoutes.Store.IncreaseProduct)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> IncreaseProduct(int id, int productId)
    {
        if (id < 1 || productId < 1) return BadRequest("Invalid id");

        Result operationResult = await _storeService.IncreaseProductAsync(productId: productId, storeId: id);

        if (!operationResult.IsSuccess) return operationResult.ToActionResult();

        return Ok("Product increase successfully");
    }

    [HttpPost]
    [Route(ApiRoutes.Store.DecreaseProduct)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DecreaseProduct(int id, int productId)
    {
        if (id < 1 || productId < 1) return BadRequest("Invalid id");

        Result operationResult = await _storeService.DecreaseProductAsync(productId: productId, storeId: id);

        if (!operationResult.IsSuccess) return operationResult.ToActionResult();

        return Ok("Product decrease successfully");
    }

    [HttpGet]
    [Route(ApiRoutes.Store.GetStoreWithProduct)]
    [AllowAnonymous]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
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

    [HttpGet]
    [AllowAnonymous]
    [Route(ApiRoutes.Store.GetStoreProductsPaginated)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(PaginatedList<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStoreWithProductPaginated([FromQuery] Pagination pagination, [FromQuery] string? nameFilter = null, [FromQuery] string? categoryFilter = null)
    {
        var productFilteredResult = await _storeService.ProductsFiltered(pagination, nameFilter, categoryFilter);

        if (!productFilteredResult.IsSuccess)
        {
            return productFilteredResult.ToActionResult();
        }

        var productFiltered = productFilteredResult.Data;
        var productFilteredList = productFiltered.Items.Select(i => _mapper.Map<ProductResponse>(i)).ToList();

        return Ok(productFiltered.To<ProductResponse, Product>(productFilteredList));
    }
}
