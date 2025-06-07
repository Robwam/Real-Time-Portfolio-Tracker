using Microsoft.AspNetCore.Mvc;
using Portfolio.Services.Interfaces;
using Shared.Models.DTOs;
using Portfolio.Converters;

namespace Portfolio.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class PortfolioController : ControllerBase
{
    private readonly IPortfolioService _portfolioService;

    public PortfolioController(IPortfolioService portfolioService)
    {
        _portfolioService = portfolioService ?? throw new ArgumentNullException(nameof(portfolioService));
    }

    /// <summary>
    /// Get a user's portfolio with all holdings and performance metrics
    /// </summary>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(PortfolioDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<PortfolioDto>> GetPortfolio(Guid userId)
    {
        var portfolio = await _portfolioService.GetPortfolioAsync(userId);
        if (portfolio is null)
            return NotFound();

        var portfolioDto = DtoConverter.ToPortfolioDto(portfolio);
        return Ok(portfolioDto);
    }

    /// <summary>
    /// Get all holdings for a user
    /// </summary>
    [HttpGet("{userId:guid}/holdings")]
    [ProducesResponseType(typeof(IEnumerable<HoldingDto>), 200)]
    [ProducesResponseType(204)]
    public async Task<ActionResult<IEnumerable<HoldingDto>>> GetHoldings(Guid userId)
    {
        var holdings = await _portfolioService.GetHoldingsAsync(userId);

        if (holdings == null || !holdings.Any())
            return NoContent();

        var holdingsDto = holdings.Select(DtoConverter.ToHoldingDto);
        return Ok(holdingsDto);
    }

    /// <summary>
    /// Get a specific holding by ID
    /// </summary>
    [HttpGet("holdings/{holdingId:guid}")]
    [ProducesResponseType(typeof(HoldingDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<HoldingDto>> GetHolding(Guid holdingId)
    {
        var holding = await _portfolioService.GetHoldingAsync(holdingId);

        if (holding is null)
            return NotFound();

        var holdingDto = DtoConverter.ToHoldingDto(holding);
        return Ok(holdingDto);
    }

    /// <summary>
    /// Add a new holding to a user's portfolio
    /// </summary>
    [HttpPost("holdings")]
    [ProducesResponseType(typeof(HoldingDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<HoldingDto>> AddHolding([FromBody] AddHoldingRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var holding = await _portfolioService.AddHoldingAsync(request);
            var holdingDto = DtoConverter.ToHoldingDto(holding);
            return CreatedAtAction(nameof(GetHolding), new { holdingId = holdingDto.Id }, holdingDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    /// <summary>
    /// Update an existing holding
    /// </summary>
    [HttpPut("holdings/{holdingId:guid}")]
    [ProducesResponseType(typeof(HoldingDto), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<HoldingDto>> UpdateHolding(Guid holdingId, [FromBody] UpdateHoldingRequestDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (holdingId != request.HoldingId)
        {
            return BadRequest($"Route ID must match {nameof(UpdateHoldingRequestDto.HoldingId)} in the request body.");
        }

        var updatedHolding = await _portfolioService.UpdateHoldingAsync(request);

        if (updatedHolding is null)
            return NotFound();

        var updatedHoldingDto = DtoConverter.ToHoldingDto(updatedHolding);
        return Ok(updatedHoldingDto);
    }

    /// <summary>
    /// Delete a holding
    /// </summary>
    [HttpDelete("holdings/{holdingId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteHolding(Guid holdingId)
    {
        var result = await _portfolioService.DeleteHoldingAsync(holdingId);
        return !result ? NotFound() : NoContent();
    }
}
