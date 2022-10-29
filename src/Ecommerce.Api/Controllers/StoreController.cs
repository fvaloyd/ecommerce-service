using AutoMapper;
using Ecommerce.Api.Dtos.Store;
using Ecommerce.Core.Consts;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Infrastructure.Persistence.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class StoreController : ApiControllerBase
{
    private readonly IStoreService _storeService;
    private readonly IEfRepository<Store> _storeRepo;
    private readonly IEfRepository<ProductStore> _productStoreRepo;
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public StoreController(
        IStoreService storeService,
        IEfRepository<Store> storeRepo,
        IMapper mapper,
        ApplicationDbContext db,
        IEfRepository<ProductStore> productStoreRepo)
    {
        _storeService = storeService;
        _storeRepo = storeRepo;
        _mapper = mapper;
        _db = db;
        _productStoreRepo = productStoreRepo;
    }

    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<Store>>> GetAllStores()
    {
        IEnumerable<Store> stores = await _storeRepo.GetAllAsync();
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
        if (!ModelState.IsValid) return BadRequest("Invalid store");

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

        storeToUpdate.Name = storeDto.Name;
        storeToUpdate.State = storeDto.State;

        _storeRepo.UpdateAsync(id, storeToUpdate);

        int result = await _storeRepo.SaveChangeAsync();

        if (result < 1) return BadRequest("Could not update the store");

        return NoContent();
    }

    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteStore(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Store storeToDelete = _storeRepo.GetFirst(x => x.Id == id);

        if (storeToDelete is null) return NotFound($"Could not found the store with the Id::{id}");

        _storeRepo.Remove(storeToDelete);

        await _storeService.DeleteProductStoreRelation(id);

        int operationResult = await _db.SaveChangesAsync();

        if (operationResult < 1) return BadRequest("Could not remove the store");

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

    [HttpPost("AddProduct")]
    public async Task<IActionResult> AddProduct(int productId, int storeId)
    {
        if (storeId < 1 || productId < 1) return BadRequest("Invalid id");

        bool operationResult = await _storeService.AddProductAsync(productId: productId, storeId: storeId);

        if (operationResult is false) return BadRequest("Could not add the product to the store");

        return Ok("Product Added successfully");
    }

    [HttpGet("GetStoreWithProducts/{id}")]
    public async Task<ActionResult<StoreWithProductDto>> GetStoreWithProducts(int id)
    {
        var store = _storeRepo.GetFirst(s => s.Id == id);

        if (store is null) return NotFound("Could not found the store");

        var productStore = await _productStoreRepo.GetAllAsync(s => s.StoreId == id, IncludeProperty: "Product");

        if (productStore is null) return NotFound("Could not found the store");

        var storeWithProductDto = _mapper.Map<StoreWithProductDto>(productStore);

        storeWithProductDto.State = store.State;
        storeWithProductDto.Name = store.Name;

        return storeWithProductDto;
    }
}
