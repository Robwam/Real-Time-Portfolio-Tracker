using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Services.Interfaces;
using Shared.Models.DTOs;

namespace Portfolio.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        
        if (portfolio == null)
            return NotFound();
            
        return Ok(portfolio);
    }

    /// <summary>
    /// Get all holdings for a user
    /// </summary>
    [HttpGet("{userId:guid}/holdings")]
    [ProducesResponseType(typeof(IEnumerable<HoldingDto>), 200)]
    public async Task<ActionResult<IEnumerable<HoldingDto>>> GetHoldings(Guid userId)
    {
        var holdings = await _portfolioService.GetHoldingsAsync(userId);
        return Ok(holdings);
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
        
        if (holding == null)
            return NotFound();
            
        return Ok(holding);
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
            return CreatedAtAction(nameof(GetHolding), new { holdingId = holding.Id }, holding);
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
            return BadRequest("Holding ID in route must match ID in request body");
            
        var updatedHolding = await _portfolioService.UpdateHoldingAsync(request);
        
        if (updatedHolding == null)
            return NotFound();
            
        return Ok(updatedHolding);
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
        
        if (!result)
            return NotFound();
            
        return NoContent();
    }
}