using Moq;
using Portfolio.Services.Interfaces;
using Portfolio.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Shared.Models.DTOs;

namespace Portfolio.Tests;

public class PortfolioControllerTests
{
    private readonly Mock<IPortfolioService> _mockService;
    private readonly PortfolioController _controller;
    private readonly HelperMethods _helperMethods;
    private readonly Portfolio.Models.Portfolio _testPortfolio;

    public PortfolioControllerTests()
    {
        _mockService = new Mock<IPortfolioService>();
        _controller = new PortfolioController(_mockService.Object);
        _helperMethods = new HelperMethods();
        _testPortfolio = _helperMethods.CreateTestPortfolio();
    }

    private void SetupMockPortfolioWithHoldings()
    {
        _mockService
            .Setup(s => s.GetPortfolioAsync(_helperMethods.TestUserId))
            .ReturnsAsync(_testPortfolio);
    }

    [Fact]
    public async Task GetPortfolio_ReturnsOk_WithPortfolio_WhenUserExists()
    {
        SetupMockPortfolioWithHoldings();

        var actionResult = await _controller.GetPortfolio(_helperMethods.TestUserId);

        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var actualPortfolio = Assert.IsType<Portfolio.Models.Portfolio>(okResult.Value);

        Assert.Equal(_testPortfolio, actualPortfolio, new PortfolioEqualityComparer());
    }

    [Fact]
    public async Task GetPortfolio_ReturnsNotFound_WhenUserDoesNotExist()
    {
        _mockService
            .Setup(s => s.GetPortfolioAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Portfolio.Models.Portfolio?)null);

        var result = await _controller.GetPortfolio(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetHoldings_ReturnsOk_WithHoldings_WhenUserHasHoldings()
    {
        var holdings = _helperMethods.CreateTestHoldings();
        _mockService.Setup(s => s.GetHoldingsAsync(_helperMethods.TestUserId))
            .ReturnsAsync(holdings);

        var result = await _controller.GetHoldings(_helperMethods.TestUserId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualHoldings = Assert.IsAssignableFrom<IEnumerable<Holding>>(okResult.Value);
        Assert.NotEmpty(actualHoldings);
    }

    [Fact]
    public async Task GetHoldings_ReturnsNoContent_WhenUserHasNoHoldings()
    {
        _mockService.Setup(s => s.GetHoldingsAsync(_helperMethods.TestUserId))
            .ReturnsAsync(new List<Holding>());

        var result = await _controller.GetHoldings(_helperMethods.TestUserId);

        Assert.IsType<NoContentResult>(result.Result);
    }

    [Fact]
    public async Task GetHolding_ReturnsOk_WithHolding_WhenHoldingExists()
    {
        var holding = _helperMethods.CreateTestHoldings()[0];
        _mockService.Setup(s => s.GetHoldingAsync(holding.Id))
            .ReturnsAsync(holding);

        var result = await _controller.GetHolding(holding.Id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualHolding = Assert.IsType<Holding>(okResult.Value);

        Assert.Equal(holding, actualHolding, new HoldingEqualityComparer());
    }

    [Fact]
    public async Task GetHolding_ReturnsNotFound_WhenHoldingDoesNotExist()
    {
        _mockService.Setup(s => s.GetHoldingAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Holding?)null);

        var result = await _controller.GetHolding(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task AddHolding_ReturnsCreatedAtAction_WithHolding_WhenValid()
    {
        var request = _helperMethods.CreateAddHoldingRequestDto();
        var createdHolding = _helperMethods.CreateTestHoldings()[0];

        _mockService.Setup(s => s.AddHoldingAsync(request))
            .ReturnsAsync(createdHolding);

        var result = await _controller.AddHolding(request);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedHolding = Assert.IsType<Holding>(createdAtActionResult.Value);
        Assert.Equal(createdHolding.Id, returnedHolding.Id);
    }

    [Fact]
    public async Task AddHolding_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        _controller.ModelState.AddModelError("Symbol", "Required");

        var result = await _controller.AddHolding(new AddHoldingRequestDto());

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task AddHolding_ReturnsBadRequest_WithMessage_WhenServiceThrowsArgumentException()
    {
        var request = _helperMethods.CreateAddHoldingRequestDto();

        _mockService.Setup(s => s.AddHoldingAsync(request))
            .ThrowsAsync(new ArgumentException("Invalid data"));

        var result = await _controller.AddHolding(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("Invalid data", badRequest.Value?.ToString() ?? string.Empty);
    }

    [Fact]
    public async Task AddHolding_ReturnsInternalServerError_WithMessage_WhenServiceThrowsInvalidOperationException()
    {
        var request = _helperMethods.CreateAddHoldingRequestDto();

        _mockService.Setup(s => s.AddHoldingAsync(request))
            .ThrowsAsync(new InvalidOperationException("Server error"));

        var result = await _controller.AddHolding(request);

        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, objectResult.StatusCode);
        Assert.Contains("Server error", objectResult.Value?.ToString() ?? string.Empty);
    }

    [Fact]
    public async Task UpdateHolding_ReturnsOk_WithUpdatedHolding_WhenValid()
    {
        var request = _helperMethods.CreateUpdateHoldingRequestDto();
        var updatedHolding = _helperMethods.CreateTestHoldings()[0];

        _mockService.Setup(s => s.UpdateHoldingAsync(request))
            .ReturnsAsync(updatedHolding);

        var result = await _controller.UpdateHolding(request.HoldingId, request);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var actualHolding = Assert.IsType<Holding>(okResult.Value);

        Assert.Equal(updatedHolding, actualHolding, new HoldingEqualityComparer());
    }

    [Fact]
    public async Task UpdateHolding_ReturnsBadRequest_WhenIdsMismatch()
    {
        var request = _helperMethods.CreateUpdateHoldingRequestDto();

        var result = await _controller.UpdateHolding(Guid.NewGuid(), request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Contains("Route ID must match", badRequest.Value?.ToString() ?? string.Empty);
    }

    [Fact]
    public async Task UpdateHolding_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
        _controller.ModelState.AddModelError("Quantity", "Required");
        var request = _helperMethods.CreateUpdateHoldingRequestDto();

        var result = await _controller.UpdateHolding(request.HoldingId, request);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateHolding_ReturnsNotFound_WhenHoldingDoesNotExist()
    {
        var request = _helperMethods.CreateUpdateHoldingRequestDto();

        _mockService.Setup(s => s.UpdateHoldingAsync(request))
            .ReturnsAsync((Holding?)null);

        var result = await _controller.UpdateHolding(request.HoldingId, request);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task DeleteHolding_ReturnsNoContent_WhenDeleteSucceeds()
    {
        var holdingId = Guid.NewGuid();

        _mockService.Setup(s => s.DeleteHoldingAsync(holdingId))
            .ReturnsAsync(true);

        var result = await _controller.DeleteHolding(holdingId);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteHolding_ReturnsNotFound_WhenHoldingDoesNotExist()
    {
        var holdingId = Guid.NewGuid();

        _mockService.Setup(s => s.DeleteHoldingAsync(holdingId))
            .ReturnsAsync(false);

        var result = await _controller.DeleteHolding(holdingId);

        Assert.IsType<NotFoundResult>(result);
    }
}
