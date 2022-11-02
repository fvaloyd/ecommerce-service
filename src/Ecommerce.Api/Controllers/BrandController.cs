using AutoMapper;
using Ecommerce.APi.Dtos.Brand;
using Ecommerce.Core.Consts;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[Authorize]
public class BrandController : ApiControllerBase
{
    private readonly IEfRepository<Brand> _brandRepo;
    private readonly IMapper _mapper;

    public BrandController(IEfRepository<Brand> brandRepo, IMapper mapper)
    {
        _brandRepo = brandRepo;
        _mapper = mapper;
    }

    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<Brand>> GetAllBrands()
    {
        IEnumerable<Brand> brands = _brandRepo.GetAll(); 
        return brands.ToList();
    }

    [HttpGet("GetById/{id}", Name = "GetBrandById")]
    public ActionResult<Brand> GetBrandById(int id)
    {
        if (id < 1)
            return BadRequest("Invalid id");

        Brand brand = _brandRepo.GetFirst(b => b.Id == id);

        if (brand is null)
            return NotFound($"Brand with the id::{id} not found");

        return brand;
    }

    [HttpPost("Create")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> CreateBrand([FromBody] PostBrandDto brandDto)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid brand");

        Brand brand = _mapper.Map<Brand>(brandDto);
        Brand brandCreated = await _brandRepo.AddAsync(brand);

        if (brandCreated is null)
            return BadRequest("Could not create the brand");

        return RedirectToRoute(nameof(GetBrandById), new {id = brandCreated.Id} );
    }

    [HttpPut("Edit/{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> EditBrand(int id, [FromBody] PutBrandDto brandDto)
    {
        if (id < 1)
            return BadRequest("Invalid id");

        Brand brandToUpdate = _brandRepo.GetFirst(b => b.Id == id);

        if (brandToUpdate is null)
            return NotFound($"Brand with the id::{id} not found");

        brandToUpdate.Name = brandDto.Name;
        brandToUpdate.State = brandDto.State;

        _brandRepo.Update(brandToUpdate);

        int result = await _brandRepo.SaveChangeAsync();

        if (result < 1)
            return BadRequest("Could not edit the brand");

        return NoContent();
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        if (id < 1)
            return BadRequest("Invalid id");

        Brand brandToDelete = _brandRepo.GetFirst(b => b.Id == id);

        if (brandToDelete is null)
            return NotFound($"Brand with the id::{id} not found");

        _brandRepo.Remove(brandToDelete);

        int result = await _brandRepo.SaveChangeAsync();

        if (result < 1)
            return BadRequest("Could not delete the brand");

        return NoContent();
    }
}