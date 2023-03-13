using Ecommerce.Core.Enums;
using Ecommerce.Core.Entities;
using Ecommerce.Application.Data;
using Ecommerce.Contracts.Categories;
using Ecommerce.Contracts.Endpoints;

using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
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

    [HttpGet]
    [Route(CategoryEndpoints.GetAllCategories)]
    [ProducesResponseType(typeof(List<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryResponse>>> Get()
    {
        List<CategoryResponse> categories = await _db.Categories
                                                .ProjectTo<CategoryResponse>(_mapper.ConfigurationProvider)
                                                .ToListAsync();

        return Ok(categories);
    }

    [HttpGet]
    [Route(CategoryEndpoints.GetCategoryById + "{id}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponse>> Get(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Category? category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category is null) return NotFound($"Category with the id::{id} not found");

        return Ok(_mapper.Map<CategoryResponse>(category));
    }

    [HttpPost]
    [Route(CategoryEndpoints.CreateCategory)]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest categoryRequest)
    {
        Category category = _mapper.Map<Category>(categoryRequest);

        await _db.Categories.AddAsync(category);

        await _db.SaveChangesAsync();

        if (category.Id < 1) return BadRequest("Could not create the category");

        return RedirectToAction("Get", new {id = category.Id} );
    }

    [HttpPut]
    [Route(CategoryEndpoints.EditCategory + "{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Edit(int id, [FromBody] EditCategoryRequest categoryRequest)
    {
        if (id < 1) return BadRequest("Invalid id");

        if (!await _db.Categories.AnyAsync(c => c.Id == id)) return NotFound($"Category with the id::{id} not found");

        Category categoryToUpdate = _mapper.Map<Category>(categoryRequest);

        categoryToUpdate.Id = id;

        _db.Categories.Update(categoryToUpdate);

        if (await _db.SaveChangesAsync() < 1) return BadRequest("Could not edit the category");

        return NoContent();
    }

    [HttpDelete]
    [Route(CategoryEndpoints.DeleteCategory + "{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id)
    {
        if (id < 1) return BadRequest("Invalid id");

        Category? category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id);

        if (category is null) return NotFound($"Category with the id::{id} not found");

        _db.Categories.Remove(category);

        if (await _db.SaveChangesAsync() < 1) return BadRequest("Could not remove the category");

        return NoContent();
    }
}