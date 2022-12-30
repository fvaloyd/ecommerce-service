using Ecommerce.Core.Enums;
using Ecommerce.Core.Entities;
using Ecommerce.APi.Dtos.Brand;
using Ecommerce.Application.Data;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Api.Controllers;

[Authorize]
public class BrandController : ApiControllerBase
{
    private readonly IEcommerceDbContext _db;
    private readonly IMapper _mapper;

    public BrandController(
        IMapper mapper,
        IEcommerceDbContext db)
    {
        _mapper = mapper;
        _db = db;
    }

    [HttpGet("GetAll")]
    [ProducesResponseType(typeof(List<Brand>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Brand>>> GetAllBrands()
    {
        List<Brand> brands = await _db.Brands.ToListAsync();

        return Ok(brands);
    }

    [HttpGet("GetById/{id}", Name = "GetBrandById")]
    [ProducesResponseType(typeof(Brand), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Brand>> GetBrandById(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Brand? brand = await _db.Brands.FirstOrDefaultAsync(b => b.Id == id);

        if (brand is null) return NotFound($"Brand with the id::{id} not found");

        return Ok(brand);
    }

    [HttpPost("Create")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(Brand), StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBrand([FromBody] CreateBrandRequest brandRequest)
    {
        Brand brand = _mapper.Map<Brand>(brandRequest);

        await _db.Brands.AddAsync(brand);

        await _db.SaveChangesAsync();

        if (brand.Id < 1) return BadRequest("Could not create the brand");

        return RedirectToRoute(nameof(GetBrandById), new { id = brand.Id });
    }

    [HttpPut("Edit/{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> EditBrand(int id, [FromBody] EditBrandRequest brandRequest)
    {
        if (id < 1) return BadRequest("Invalid id");

        if (!await _db.Brands.AnyAsync(b => b.Id == id)) return NotFound($"Brand with the id::{id} not found");

        Brand brandToUpdate = _mapper.Map<Brand>(brandRequest);
        brandToUpdate.Id = id;

        _db.Brands.Update(brandToUpdate);

        if (await _db.SaveChangesAsync() < 1) return BadRequest("Could not edit the brand");

        return NoContent();
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Brand? brandToDelete = await _db.Brands.FirstOrDefaultAsync(b => b.Id == id);

        if (brandToDelete is null) return NotFound($"Brand with the id::{id} not found");

        _db.Brands.Remove(brandToDelete);

        if (await _db.SaveChangesAsync() < 1) return BadRequest("Could not delete the brand");

        return NoContent();
    }
}