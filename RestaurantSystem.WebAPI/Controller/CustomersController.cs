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
    public async Task<IActionResult> CreateProfile([FromBody] CreateCustomerProfileCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Customer profile created" });
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateCustomerProfileCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Customer profile updated" });
    }

    [HttpPost("profile/image")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadProfileImage([FromForm] UploadProfileImageRequest request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest(new { errors = new[] { "File is required" } });
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
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { profileImageUrl = result.Data });
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var result = await _mediator.Send(new GetCustomerProfileQuery());
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost("addresses")]
    public async Task<IActionResult> AddAddress([FromBody] AddAddressCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Address added" });
    }

    [HttpPut("addresses/{addressId:int}")]
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
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Address updated" });
    }

    [HttpDelete("addresses/{addressId:int}")]
    public async Task<IActionResult> DeleteAddress(int addressId)
    {
        var result = await _mediator.Send(new DeleteAddressCommand(addressId));
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(new { message = "Address deleted" });
    }

    [HttpGet("addresses")]
    public async Task<IActionResult> GetAddresses()
    {
        var result = await _mediator.Send(new GetCustomerAddressesQuery());
        return Ok(result);
    }

    [HttpGet("loyalty")]
    public async Task<IActionResult> GetLoyaltyPoints()
    {
        var result = await _mediator.Send(new GetLoyaltyPointsQuery());
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
