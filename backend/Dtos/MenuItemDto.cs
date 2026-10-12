using System.ComponentModel.DataAnnotations;

namespace CampusEats.Api.Dtos;

public record MenuItemDto(int Id, string Name, decimal Price, string Category, bool Available);

public record CreateMenuItemDto(
    [Required] string Name,
    [Range(0.01, 100000)] decimal Price,
    [Required] string Category
);
