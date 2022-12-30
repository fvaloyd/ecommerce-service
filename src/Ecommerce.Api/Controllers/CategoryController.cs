using Ecommerce.Core.Enums;
using Ecommerce.Core.Entities;
using Ecommerce.Application.Data;
using Ecommerce.Api.Dtos.Category;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Ecommerce.Api.Controllers;

[Authorize]
public class CategoryController : ApiControllerBase
{
    private readonly IEcommerceDbContext _db;
    private readonly IMapper _mapper;

    public CategoryController(
        IMapper mapper,
        IEcommerceDbContext db)
    {
        _mapper = mapper;
        _db = db;
    }

    [HttpGet("GetAll")]
    [ProducesResponseType(typeof(List<Category>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<Category>>> GetAllCategories()
    {
        List<Category> categories = await _db.Categories.ToListAsync();

        return Ok(categories);
    }

    [HttpGet("GetById/{id}", Name = "GetCategoryById")]
    [ProducesResponseType(typeof(Category), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Category>> GetCategoryById(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Category? category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category is null) return NotFound($"Category with the id::{id} not found");

        return Ok(category);
    }

    [HttpPost("Create")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(Category), StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest categoryRequest)
    {
        Category category = _mapper.Map<Category>(categoryRequest);

        await _db.Categories.AddAsync(category);

        await _db.SaveChangesAsync();

        if (category.Id < 1) return BadRequest("Could not create the category");

        return RedirectToRoute(nameof(GetCategoryById), new {id = category.Id} );
    }

    [HttpPut("Edit/{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> EditCategory(int id, [FromBody] EditCategoryRequest categoryRequest)
    {
        if (id < 1) return BadRequest("Invalid id");

        if (!await _db.Categories.AnyAsync(c => c.Id == id)) return NotFound($"Category with the id::{id} not found");

        Category categoryToUpdate = _mapper.Map<Category>(categoryRequest);

        categoryToUpdate.Id = id;

        _db.Categories.Update(categoryToUpdate);

        if (await _db.SaveChangesAsync() < 1) return BadRequest("Could not edit the category");

        return NoContent();
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Category? category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category is null) return NotFound($"Category with the id::{id} not found");

        _db.Categories.Remove(category);

        if (await _db.SaveChangesAsync() < 1) return BadRequest("Could not remove the category");

        return NoContent();
    }
}