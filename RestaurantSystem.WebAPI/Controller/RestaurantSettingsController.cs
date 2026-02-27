using System.Linq;
using System.Text.Json;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantSystem.Application.Common.Interfaces;
using RestaurantSystem.Application.Features.RestaurantSettings.Commands.UpsertRestaurantSetting;
using RestaurantSystem.Application.Features.RestaurantSettings.DTOs;
using RestaurantSystem.Application.Features.RestaurantSettings.Queries.GetRestaurantSetting;
using RestaurantSystem.Domain.Enums;
using RestaurantSystem.WebAPI.Helpers;

namespace RestaurantSystem.WebAPI.Controllers;

[ApiController]
[Route("api/admin/restaurant")]
[Authorize(Roles = "Admin,Manager")]
public class RestaurantSettingsController : ControllerBase
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private const string IdentityKey = "Identity";
    private const string OpeningHoursKey = "OpeningHours";
    private const string DeliveryKey = "Delivery";
    private const string TaxFeesKey = "TaxFees";
    private const string PaymentMethodsKey = "PaymentMethods";

    private readonly IMediator _mediator;
    private readonly IOrderSettings _orderSettings;

    public RestaurantSettingsController(IMediator mediator, IOrderSettings orderSettings)
    {
        _mediator = mediator;
        _orderSettings = orderSettings;
    }

    [HttpGet("identity")]
    public async Task<ActionResult<RestaurantIdentityDto>> GetIdentity()
    {
        var dto = await GetSettingAsync<RestaurantIdentityDto>(IdentityKey) ?? new RestaurantIdentityDto();
        return Ok(dto);
    }

    [HttpPut("identity")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateIdentity([FromBody] RestaurantIdentityDto dto)
    {
        return await UpsertSettingAsync(IdentityKey, dto);
    }

    [HttpGet("opening-hours")]
    public async Task<ActionResult<OpeningHoursDto>> GetOpeningHours()
    {
        var dto = await GetSettingAsync<OpeningHoursDto>(OpeningHoursKey) ?? GetDefaultOpeningHours();
        return Ok(dto);
    }

    [HttpPut("opening-hours")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateOpeningHours([FromBody] OpeningHoursDto dto)
    {
        return await UpsertSettingAsync(OpeningHoursKey, dto);
    }

    [HttpGet("delivery")]
    public async Task<ActionResult<DeliverySettingsDto>> GetDelivery()
    {
        var dto = await GetSettingAsync<DeliverySettingsDto>(DeliveryKey) ?? new DeliverySettingsDto
        {
            Fee = _orderSettings.DeliveryFee
        };
        return Ok(dto);
    }

    [HttpPut("delivery")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateDelivery([FromBody] DeliverySettingsDto dto)
    {
        return await UpsertSettingAsync(DeliveryKey, dto);
    }

    [HttpGet("tax-fees")]
    public async Task<ActionResult<TaxFeesDto>> GetTaxFees()
    {
        var dto = await GetSettingAsync<TaxFeesDto>(TaxFeesKey) ?? new TaxFeesDto
        {
            TaxRate = _orderSettings.TaxRate,
            ServiceFeeRate = 0m
        };
        return Ok(dto);
    }

    [HttpPut("tax-fees")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateTaxFees([FromBody] TaxFeesDto dto)
    {
        return await UpsertSettingAsync(TaxFeesKey, dto);
    }

    [HttpGet("payment-methods")]
    public async Task<ActionResult<PaymentMethodsDto>> GetPaymentMethods()
    {
        var dto = await GetSettingAsync<PaymentMethodsDto>(PaymentMethodsKey) ?? new PaymentMethodsDto
        {
            EnabledMethods = Enum.GetValues<PaymentMethod>().ToList()
        };
        return Ok(dto);
    }

    [HttpPut("payment-methods")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdatePaymentMethods([FromBody] PaymentMethodsDto dto)
    {
        return await UpsertSettingAsync(PaymentMethodsKey, dto);
    }

    private async Task<T?> GetSettingAsync<T>(string key)
    {
        var value = await _mediator.Send(new GetRestaurantSettingQuery(key));
        if (string.IsNullOrWhiteSpace(value))
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(value, JsonOptions);
    }

    private async Task<IActionResult> UpsertSettingAsync<T>(string key, T dto)
    {
        var json = JsonSerializer.Serialize(dto, JsonOptions);
        var result = await _mediator.Send(new UpsertRestaurantSettingCommand
        {
            Key = key,
            Value = json
        });

        if (!result.Succeeded)
        {
            return this.ToValidationProblem(result.Errors);
        }

        return NoContent();
    }

    private static OpeningHoursDto GetDefaultOpeningHours()
    {
        var days = Enum.GetValues<DayOfWeek>()
            .Select(d => new OpeningDayDto
            {
                DayOfWeek = d,
                IsClosed = true,
                Ranges = new List<TimeRangeDto>()
            })
            .ToList();

        return new OpeningHoursDto { Days = days };
    }
}
