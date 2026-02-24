using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Features.Menu.Commands.AddOption;
using RestaurantSystem.Application.Features.Menu.Commands.CreateCategory;
using RestaurantSystem.Application.Features.Menu.Commands.CreateMenuItem;
using RestaurantSystem.Application.Features.Menu.Commands.CreateOptionGroup;
using RestaurantSystem.Application.Features.Menu.Commands.DeleteMenuItem;
using RestaurantSystem.Application.Features.Menu.Commands.UpdateMenuItem;
using RestaurantSystem.Application.Features.Menu.Commands.UploadCategoryImage;
using RestaurantSystem.Application.Features.Menu.Commands.UploadMenuItemImage;
using RestaurantSystem.Application.Features.Menu.Queries.GetCategories;
using RestaurantSystem.Application.Features.Menu.Queries.GetFeaturedItems;
using RestaurantSystem.Application.Features.Menu.Queries.GetMenuItemById;
using RestaurantSystem.Application.Features.Menu.Queries.GetMenuItems;
using RestaurantSystem.WebAPI.Models;
using RestaurantSystem.WebAPI.Helpers;
using RestaurantSystem.WebAPI.Models.Responses;
using RestaurantSystem.Application.Features.Menu.DTOs;

namespace RestaurantSystem.WebAPI.Controllers;

[ApiController]
[Route("api/menu")]
public class MenuController : ControllerBase
{
    private readonly IMediator _mediator;

    public MenuController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("items")]
    [AllowAnonymous]
    public async Task<ActionResult<List<MenuItemDto>>> GetMenuItems([FromQuery] GetMenuItemsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("items/{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<MenuItemDetailDto>> GetMenuItemById(int id)
    {
        var result = await _mediator.Send(new GetMenuItemByIdQuery(id));
        if (result == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Menu item not found");
        }

        return Ok(result);
    }

    [HttpGet("categories")]
    [AllowAnonymous]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories([FromQuery] GetCategoriesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("featured")]
    [AllowAnonymous]
    public async Task<ActionResult<List<MenuItemDto>>> GetFeaturedItems()
    {
        var result = await _mediator.Send(new GetFeaturedItemsQuery());
        return Ok(result);
    }

    [HttpPost("categories")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MessageResponseDto>> CreateCategory([FromBody] CreateCategoryCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new MessageResponseDto { Message = "Category created" });
    }

    [HttpPost("categories/{categoryId:int}/image")]
    [Authorize(Roles = "Admin,Manager")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<ActionResult<UploadImageResponseDto>> UploadCategoryImage(int categoryId, [FromForm] UploadCategoryImageRequest request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return this.ToValidationProblem("File is required");
        }

        using var ms = new MemoryStream();
        await request.File.CopyToAsync(ms);

        var command = new UploadCategoryImageCommand
        {
            CategoryId = categoryId,
            Content = ms.ToArray(),
            FileName = request.File.FileName,
            ContentType = request.File.ContentType
        };

        var result = await _mediator.Send(command);
        if (!result.Succeeded || result.Data == null)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new UploadImageResponseDto { ImageUrl = result.Data });
    }

    [HttpPost("items")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MessageResponseDto>> CreateMenuItem([FromBody] CreateMenuItemCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new MessageResponseDto { Message = "Menu item created" });
    }

    [HttpPut("items/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateMenuItem(int id, [FromBody] UpdateMenuItemRequest request)
    {
        var command = new UpdateMenuItemCommand
        {
            Id = id,
            CategoryId = request.CategoryId,
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            ImageUrl = request.ImageUrl,
            ThumbnailUrl = request.ThumbnailUrl,
            IsAvailable = request.IsAvailable,
            IsFeatured = request.IsFeatured,
            PreparationTimeMinutes = request.PreparationTimeMinutes,
            Calories = request.Calories,
            SpiceLevel = request.SpiceLevel,
            IsVegetarian = request.IsVegetarian,
            IsVegan = request.IsVegan,
            IsGlutenFree = request.IsGlutenFree,
            IsDairyFree = request.IsDairyFree,
            IsNutFree = request.IsNutFree,
            AllergenInfo = request.AllergenInfo,
            MaxQuantityPerOrder = request.MaxQuantityPerOrder
        };

        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpDelete("items/{id:int}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteMenuItem(int id)
    {
        var result = await _mediator.Send(new DeleteMenuItemCommand(id));
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpPost("items/{itemId:int}/option-groups")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MessageResponseDto>> CreateOptionGroup(int itemId, [FromBody] CreateOptionGroupCommand command)
    {
        var updated = command with { ItemId = itemId };
        var result = await _mediator.Send(updated);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new MessageResponseDto { Message = "Option group created" });
    }

    [HttpPost("option-groups/{groupId:int}/options")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult<MessageResponseDto>> AddOption(int groupId, [FromBody] AddOptionCommand command)
    {
        var updated = command with { OptionGroupId = groupId };
        var result = await _mediator.Send(updated);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new MessageResponseDto { Message = "Option added" });
    }

    [HttpPost("items/{itemId:int}/image")]
    [Authorize(Roles = "Admin,Manager")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<ActionResult<UploadImageResponseDto>> UploadMenuItemImage(int itemId, [FromForm] UploadMenuItemImageRequest request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return this.ToValidationProblem("File is required");
        }

        using var ms = new MemoryStream();
        await request.File.CopyToAsync(ms);

        var command = new UploadMenuItemImageCommand
        {
            ItemId = itemId,
            Content = ms.ToArray(),
            FileName = request.File.FileName,
            ContentType = request.File.ContentType
        };

        var result = await _mediator.Send(command);
        if (!result.Succeeded || result.Data == null)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new UploadImageResponseDto { ImageUrl = result.Data });
    }
}
