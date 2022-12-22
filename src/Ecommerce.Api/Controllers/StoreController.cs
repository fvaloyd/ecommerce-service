using AutoMapper;
using Ecommerce.Api.Dtos.Store;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Application.Stores;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class StoreController : ApiControllerBase
{
    private readonly IStoreService _storeService;
    private readonly IEfRepository<Store> _storeRepo;
    private readonly IEfRepository<ProductStore> _productStoreRepo;
    private readonly IDbContext _db;
    private readonly IMapper _mapper;

    public StoreController(
        IStoreService storeService,
        IEfRepository<Store> storeRepo,
        IMapper mapper,
        IDbContext db,
        IEfRepository<ProductStore> productStoreRepo)
    {
        _storeService = storeService;
        _storeRepo = storeRepo;
        _mapper = mapper;
        _db = db;
        _productStoreRepo = productStoreRepo;
    }

    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<Store>> GetAllStores()
    {
        IEnumerable<Store> stores = _storeRepo.GetAll();
        return stores.ToList();
    }

    [HttpGet("GetById/{id}", Name = "GetStoreById")]
    public ActionResult<Store> GetStoreById(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Store store = _storeRepo.GetFirst(x => x.Id == id);

        if (store is null) return NotFound($"Could not found the store with the Id::{id}");

        return store;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateStore([FromBody] PostStoreDto storeDto)
    {
        Store store = _mapper.Map<Store>(storeDto);
        Store storeCreated = await _storeRepo.AddAsync(store);
        if (storeCreated is null) return BadRequest("Could not create the store");

        return RedirectToRoute(nameof(GetStoreById), new { id = storeCreated.Id });
    }

    [HttpPut("Edit/{id}")]
    public async Task<IActionResult> EditStore(int id, PutStoreDto storeDto)
    {
        if (id < 1) return BadRequest("Invalid id");

        Store storeToUpdate = _storeRepo.GetFirst(x => x.Id == id);

        if (storeToUpdate is null) return NotFound($"Could not found the store with the Id::{id}");

        _mapper.Map(storeDto, storeToUpdate);

        _storeRepo.Update(storeToUpdate);

        int rowsAffect = await _storeRepo.SaveChangeAsync();

        if (rowsAffect < 1) return BadRequest("Could not update the store");

        return NoContent();
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteStore(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Store storeToDelete = _storeRepo.GetFirst(x => x.Id == id);

        if (storeToDelete is null) return NotFound($"Could not found the store with the Id::{id}");

        _storeRepo.Remove(storeToDelete);

        _storeService.DeleteProductStoreRelation(id);

        int rowsAffect = await _db.SaveChangesAsync();

        if (rowsAffect < 1) return BadRequest("Could not remove the store");

        return NoContent();
    }

    [HttpPost("IncreaseProduct")]
    public async Task<IActionResult> IncreaseProduct(int storeId, int productId)
    {
        if (storeId < 1 || productId < 1) return BadRequest("Invalid id");

        bool operationResult = await _storeService.IncreaseProductAsync(productId: productId, storeId: storeId);

        if (operationResult is false) return BadRequest("Could not increase the product");

        return Ok("Product increase successfully");
    }

    [HttpPost("DecreaseProduct")]
    public async Task<IActionResult> DecreaseProduct(int storeId, int productId)
    {
        if (storeId < 1 || productId < 1) return BadRequest("Invalid id");

        bool operationResult = await _storeService.DecreaseProductAsync(productId: productId, storeId: storeId);

        if (operationResult is false) return BadRequest("Could not decrease the product");

        return Ok("Product decrease successfully");
    }

    [HttpGet("GetStoreWithProducts/{id}")]
    public ActionResult<StoreWithProductDto> GetStoreWithProducts(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        var store = _storeRepo.GetFirst(s => s.Id == id);

        if (store is null) return NotFound("Could not found the store");

        var productStore = _productStoreRepo.GetAll(s => s.StoreId == id, IncludeProperty: "Product");

        if (productStore is null) return NotFound("Could not found the store");

        var storeWithProductDto = _mapper.Map<StoreWithProductDto>((productStore, store));

        return storeWithProductDto;
    }
}
