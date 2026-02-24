using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Features.Customers.Commands.AddAddress;
using RestaurantSystem.Application.Features.Customers.Commands.CreateCustomerProfile;
using RestaurantSystem.Application.Features.Customers.Commands.DeleteAddress;
using RestaurantSystem.Application.Features.Customers.Commands.UpdateAddress;
using RestaurantSystem.Application.Features.Customers.Commands.UpdateCustomerProfile;
using RestaurantSystem.Application.Features.Customers.Commands.UploadProfileImage;
using RestaurantSystem.Application.Features.Customers.Queries.GetCustomerAddresses;
using RestaurantSystem.Application.Features.Customers.Queries.GetCustomerProfile;
using RestaurantSystem.Application.Features.Customers.Queries.GetLoyaltyPoints;
using RestaurantSystem.WebAPI.Models;
using RestaurantSystem.WebAPI.Helpers;
using RestaurantSystem.WebAPI.Models.Responses;
using RestaurantSystem.Application.Features.Customers.DTOs;

namespace RestaurantSystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("profile")]
    public async Task<ActionResult<MessageResponseDto>> CreateProfile([FromBody] CreateCustomerProfileCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new MessageResponseDto { Message = "Customer profile created" });
    }

    [HttpPut("profile")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerProfileCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpPost("profile/image")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<ActionResult<UploadImageResponseDto>> UploadProfileImage([FromForm] UploadProfileImageRequest request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return this.ToValidationProblem("File is required");
        }

        using var ms = new MemoryStream();
        await request.File.CopyToAsync(ms);

        var command = new UploadProfileImageCommand
        {
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

    [HttpGet("profile")]
    public async Task<ActionResult<CustomerProfileDto>> GetProfile()
    {
        var result = await _mediator.Send(new GetCustomerProfileQuery());
        if (result == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Customer profile not found");
        }

        return Ok(result);
    }

    [HttpPost("addresses")]
    public async Task<ActionResult<MessageResponseDto>> AddAddress([FromBody] AddAddressCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return Ok(new MessageResponseDto { Message = "Address added" });
    }

    [HttpPut("addresses/{addressId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateAddress(int addressId, [FromBody] UpdateAddressRequest request)
    {
        var command = new UpdateAddressCommand
        {
            AddressId = addressId,
            Label = request.Label,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            DeliveryInstructions = request.DeliveryInstructions,
            IsDefault = request.IsDefault
        };

        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpDelete("addresses/{addressId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteAddress(int addressId)
    {
        var result = await _mediator.Send(new DeleteAddressCommand(addressId));
        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    [HttpGet("addresses")]
    public async Task<ActionResult<List<CustomerAddressDto>>> GetAddresses()
    {
        var result = await _mediator.Send(new GetCustomerAddressesQuery());
        return Ok(result);
    }

    [HttpGet("loyalty")]
    public async Task<ActionResult<LoyaltyPointsDto>> GetLoyaltyPoints()
    {
        var result = await _mediator.Send(new GetLoyaltyPointsQuery());
        if (result == null)
        {
            return Problem(statusCode: StatusCodes.Status404NotFound, title: "Loyalty points not found");
        }

        return Ok(result);
    }
}
