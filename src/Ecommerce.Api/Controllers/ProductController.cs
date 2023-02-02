using Ecommerce.Core.Enums;
using Ecommerce.Core.Entities;
using Ecommerce.Api.Dtos.Product;
using Ecommerce.Application.Data;
using Ecommerce.Application.Products;
using Ecommerce.Infrastructure.CloudImageStorage;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Francisvac.Result;
using AutoMapper.QueryableExtensions;

namespace Ecommerce.Api.Controllers;

[Authorize]
public class ProductController : ApiControllerBase
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    private readonly ICloudinaryService _cloudinaryService;
    private readonly IEcommerceDbContext _db;

    public ProductController(
        IProductService productService,
        IMapper mapper,
        ICloudinaryService cloudinaryService,
        IEcommerceDbContext db)
    {
        _productService = productService;
        _mapper = mapper;
        _cloudinaryService = cloudinaryService;
        _db = db;
    }

    [HttpGet("GetAll")]
    [ProducesResponseType(typeof(List<Product>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProductResponse>>> GetAllProducts()
    {
        // var productsDto = await _db.Products.Include(p => p.Category).Include(p => p.Brand).Select(p => _mapper.Map<ProductResponse>(p)).ToListAsync();
        var productsDto = await _db.Products.Include(p => p.Category).Include(p => p.Brand).ProjectTo<ProductResponse>(_mapper.ConfigurationProvider).ToListAsync();
        return productsDto;
    }

    [HttpGet("GetById/{id}", Name = "GetProductById")]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Product), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetProductById(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Product? product = await _db.Products.Include(p => p.Brand).Include(p => p.Category).FirstOrDefaultAsync(x => x.Id == id);

        if (product is null) return NotFound($"Product with the id::{id} not found");

        return _mapper.Map<ProductResponse>(product);
    }

    [HttpPost("Create")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Product), StatusCodes.Status302Found)]
    public async Task<IActionResult> CreateProduct([FromForm] CreateProductRequest productDto)
    {
        if (productDto.StoreId < 1) return BadRequest("Need to provide the Id of the store to which this product belongs");

        Product product = _mapper.Map<Product>(productDto);

        var (productImg, publicId) = await _cloudinaryService.UploadImage(file: productDto.File, imageName: product.Name.Replace(' ', '-'));

        product.SetImage(productImg);

        await _db.Products.AddAsync(product);

        if (await _db.SaveChangesAsync() < 1) return BadRequest("Could not create the product");

        Result relatedToStoreResult = await _productService.RelatedToStoreAsync(product.Id, productDto.StoreId);

        return !relatedToStoreResult.IsSuccess
          ? relatedToStoreResult.ToActionResult()
          : (IActionResult)RedirectToRoute(nameof(GetProductById), new { id = product.Id });
    }

    [HttpPut("Edit/{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> EditProduct(int id, [FromBody] EditProductRequest productDto)
    {
        if (id < 1) return BadRequest("Invalid id");

        Product? productToEdit = await _db.Products.FirstOrDefaultAsync(x => x.Id == id);

        if (productToEdit is null) return NotFound($"Product with Id::{id} not found");

        _mapper.Map(productDto, productToEdit);

        _db.Products.Update(productToEdit);

        if (await _db.SaveChangesAsync() < 1) return BadRequest("Could not edit the product");

        return Ok("Product edited successfully");
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Product? productToDelete = await _db.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (productToDelete is null) return NotFound($"Could not found the product with the id::{id}");

        _db.Products.Remove(productToDelete);

        Result deleteProductStoreRelationResult = await _productService.DeleteProductStoreRelation(id);

        if (!deleteProductStoreRelationResult.IsSuccess) return deleteProductStoreRelationResult.ToActionResult();

        await _cloudinaryService.DeleteImage(productToDelete.Name.Replace(' ', '-'));

        return Ok("Product deleted successfully");
    }
}