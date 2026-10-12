using CampusEats.Api.Dtos;
using CampusEats.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusEats.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<MenuItemDto>> GetAll()
    {
        return Ok(_menuService.GetAll());
    }

    [HttpGet("{id}")]
    public ActionResult<MenuItemDto> GetById(int id)
    {
        var item = _menuService.GetById(id);
        if (item is null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public ActionResult<MenuItemDto> Create(CreateMenuItemDto dto)
    {
        var item = _menuService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Update(int id, CreateMenuItemDto dto)
    {
        if (!_menuService.Update(id, dto)) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(int id)
    {
        if (!_menuService.Delete(id)) return NotFound();
        return NoContent();
    }
}
