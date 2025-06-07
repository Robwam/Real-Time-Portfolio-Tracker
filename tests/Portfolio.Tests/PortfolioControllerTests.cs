using Moq;
using Portfolio.Services;
using Portfolio.Data.Repository.Interfaces;

namespace Portfolio.Tests;

public class PortfolioControllerTests
{
    
    [Fact]
    public async Task GetPortfolio_ReturnsPortfolio_WhenUserExists()
    {
    }

    [Fact]
    public async Task GetPortfolio_ReturnsNotFound_WhenUserNotFound()
    {
    }
    
    [Fact]
    public async Task GetHoldings_ReturnsHoldings_WhenUserHasHoldings()
    {
    }

    [Fact]
    public async Task GetHoldings_ReturnsOkWithEmptyList_WhenUserHasNoHoldings()
    {
    }

    [Fact]
    public async Task GetHolding_ReturnsHolding_WhenHoldingExists()
    {
    }

    [Fact]
    public async Task GetHolding_ReturnsNotFound_WhenHoldingNotFound()
    {
    }

    [Fact]
    public async Task AddHolding_ReturnsCreated_WhenHoldingIsValid()
    {
    }

    [Fact]
    public async Task AddHolding_ReturnsBadRequest_WhenModelStateIsInvalid()
    {
    }

    [Fact]
    public async Task AddHolding_ReturnsBadRequest_WhenServiceThrowsArgumentException()
    {
    }

    [Fact]
    public async Task AddHolding_ReturnsServerError_WhenServiceThrowsInvalidOperationException()
    {
    }

    [Fact]
    public async Task UpdateHolding_ReturnsUpdatedHolding_WhenValid()
    {
    }

    [Fact]
    public async Task UpdateHolding_ReturnsBadRequest_WhenIdsMismatch()
    {
    }

    [Fact]
    public async Task UpdateHolding_ReturnsBadRequest_WhenModelStateInvalid()
    {
    }

    [Fact]
    public async Task UpdateHolding_ReturnsNotFound_WhenHoldingNotFound()
    {
    }

    [Fact]
    public async Task DeleteHolding_ReturnsNoContent_WhenDeleted()
    {
    }

    [Fact]
    public async Task DeleteHolding_ReturnsNotFound_WhenHoldingDoesNotExist()
    {
    }
}

