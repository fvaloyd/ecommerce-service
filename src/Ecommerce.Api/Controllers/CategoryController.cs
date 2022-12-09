using AutoMapper;
using Ecommerce.Api.Dtos.Category;
using Ecommerce.Application.Common.Interfaces;
using Ecommerce.Core.Entities;
using Ecommerce.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[Authorize]
public class CategoryController : ApiControllerBase
{
    private readonly IEfRepository<Category> _categoryRepo;
    private readonly IMapper _mapper;

    public CategoryController(IEfRepository<Category> categoryRepo, IMapper mapper)
    {
        _categoryRepo = categoryRepo;
        _mapper = mapper;
    }

    [HttpGet("GetAll")]
    public ActionResult<IEnumerable<Category>> GetAllCategories()
    {
        IEnumerable<Category> categories = _categoryRepo.GetAll();
        return categories.ToList();
    }

    [HttpGet("GetById/{id}", Name = "GetCategoryById")]
    public ActionResult<Category> GetCategoryById(int id)
    {
        if (id < 1)
            return BadRequest("Invalid id");

        Category category = _categoryRepo.GetFirst(x => x.Id == id);

        if (category is null)
            return NotFound($"Category with the id::{id} not found");

        return category;
    }

    [HttpPost("Create")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> CreateCategory([FromBody] PostCategoryDto categoryDto)
    {
        Category category = _mapper.Map<Category>(categoryDto);

        Category categoryAdded = await _categoryRepo.AddAsync(category);

        if (categoryAdded is null)
            return BadRequest("Could not create the category");

        return RedirectToRoute(nameof(GetCategoryById), new {id = categoryAdded.Id} );
    }

    [HttpPut("Edit/{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> EditCategory(int id, [FromBody] PutCategoryDto categoryDto)
    {
        if (id < 1)
            return BadRequest("Invalid id");

        Category categoryToEdit = _categoryRepo.GetFirst(x => x.Id == id);

        if (categoryToEdit is null)
            return NotFound($"Category with the id::{id} not found");

        _mapper.Map(categoryDto, categoryToEdit);

        int result = await _categoryRepo.SaveChangeAsync();

        if (result < 1)
            return BadRequest("Could not edit the category");

        return NoContent();
    }

    [HttpDelete("Delete/{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        if (id < 1)
            return BadRequest("Invalid id");

        Category categoryToDelete = _categoryRepo.GetFirst(x => x.Id == id);

        if (categoryToDelete is null)
            return NotFound($"Category with the id::{id} not found");

        _categoryRepo.Remove(categoryToDelete);

        int result = await _categoryRepo.SaveChangeAsync();

        if (result < 1)
            return BadRequest("Could not remove the category");

        return NoContent();
    }
}