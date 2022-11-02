using System.Net;
using AutoMapper;
using Ecommerce.Api.Dtos.Product;
using Ecommerce.Core.Consts;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Ecommerce.Core.Models;
using Ecommerce.Infrastructure.Persistence;
using Ecommerce.Infrastructure.Persistence.Identity;
using Ecommerce.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[Authorize]
public class ProductController : ApiControllerBase
{
    private readonly IProductService _productService;
    private readonly IEfRepository<Product> _productRepo;
    private readonly IMapper _mapper;
    private readonly IDbContext _db;
    private readonly ICloudinaryService _cloudinaryService;

    public ProductController(
        IProductService productService,
        IMapper mapper,
        IEfRepository<Product> productRepo,
        IDbContext db,
        ICloudinaryService cloudinaryService)
    {
        _productService = productService;
        _mapper = mapper;
        _productRepo = productRepo;
        _db = db;
        _cloudinaryService = cloudinaryService;
    }

    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<GetProductDto>> GetAllProducts()
    {
        IEnumerable<Product> products = _productRepo.GetAll(IncludeProperty: "Brand,Category");
        IEnumerable<GetProductDto> productsDto = products.Select(p => _mapper.Map<GetProductDto>(p));
        return productsDto.ToList();
    }

    [HttpGet("GetById/{id}", Name = "GetProductById")]
    public ActionResult<GetProductDto> GetProductById(int id)
    {
        if (id < 1) return BadRequest(new Response(Status: HttpStatusCode.BadRequest, Message: "Invalid id"));

        Product product = _productRepo.GetFirst(x => x.Id == id, IncludeProperty: "Brand,Category");

        if (product is null) return NotFound(new Response(Status: HttpStatusCode.NotFound, Message: $"Product with the id::{id} not found"));

        GetProductDto productDto = _mapper.Map<GetProductDto>(product);

        return productDto;
    }

    [HttpPost("Create")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> CreateProduct([FromForm] PostProductDto productDto)
    {
        if (!ModelState.IsValid) return BadRequest(new Response(Status: HttpStatusCode.BadRequest, Message: "Invalid product"));

        if (productDto.StoreId == 0) return BadRequest(new Response(Status: HttpStatusCode.BadRequest, Message: "Need to provide the Id of the store to which this product belongs"));

        Product product = _mapper.Map<Product>(productDto);

        var (productImg, publicId) = await _cloudinaryService.UploadImage(file: productDto.File, imageName: product.Name.Replace(' ', '-'));

        product.ImageUrl = productImg;

        Product productInserted = await _productRepo.AddAsync(product);

        if (productInserted is null) return BadRequest(new Response(Status: HttpStatusCode.BadRequest, Message: "Could not create the product"));

        int relatedToStoreResult = await _productService.RelatedToStoreAsync(productInserted.Id, productDto.StoreId);

        if (relatedToStoreResult < 1) return BadRequest(new Response(Status: HttpStatusCode.BadRequest, Message: "Product was created but could not related with a store"));

        return RedirectToRoute(nameof(GetProductById), new { id = productInserted.Id });
    }

    [HttpPut("Edit/{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> EditProduct(int id, [FromBody] PutProductDto productDto)
    {
        if (id < 1) return BadRequest(new Response(Status: HttpStatusCode.BadRequest, Message: "Invalid id"));

        Product productToEdit = _productRepo.GetFirst(x => x.Id == id);

        if (productToEdit is null) return NotFound(new Response(Status: HttpStatusCode.NotFound, Message: $"Product with Id::{id} not found"));

        _mapper.Map(productDto, productToEdit);

        _productRepo.Update(id, productToEdit);

        int result = await _productRepo.SaveChangeAsync();

        if (result < 1) return BadRequest(new Response(Status: HttpStatusCode.BadRequest, Message: "Could not edit the product"));

        return Ok(new Response(Status: HttpStatusCode.OK, Message: "Product edited successfully"));
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        if (id < 1) return BadRequest(new Response(Status: HttpStatusCode.BadRequest, Message: "Invalid id"));

        Product productToDelete = _productRepo.GetFirst(p => p.Id == id);

        if (productToDelete is null) return NotFound(new Response(Status: HttpStatusCode.NotFound, Message: $"Could not found the product with the id::{id}"));

        _productRepo.Remove(productToDelete);

        _productService.DeleteProductStoreRelation(id);

        await _cloudinaryService.DeleteImage(productToDelete.Name.Replace(' ', '-'));

        int operationResult = await _db.SaveChangesAsync();

        if (operationResult < 1) return BadRequest(new Response(Status: HttpStatusCode.BadRequest, Message: $"Could not delete the product with Id::{id}"));

        return Ok(new Response(Status: HttpStatusCode.OK, Message: "Product deleted successfully"));
    }
}