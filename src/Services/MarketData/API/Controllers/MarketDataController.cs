using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MarketData.Application.Interfaces;
using MarketData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.Models.Enums;

namespace MarketData.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarketDataController : ControllerBase
    {
        private readonly IMarketDataService _marketDataService;
        private readonly ILogger<MarketDataController> _logger;

        public MarketDataController(
            IMarketDataService marketDataService,
            ILogger<MarketDataController> logger)
        {
            _marketDataService = marketDataService;
            _logger = logger;
        }

        [HttpGet("price/{symbol}")]
        public async Task<ActionResult<AssetPrice>> GetPrice(
            string symbol, 
            [FromQuery] AssetType assetType = AssetType.Stock,
            [FromQuery] bool refresh = false,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting price for {Symbol} ({AssetType})", symbol, assetType);
            
            var price = await _marketDataService.GetAssetPriceAsync(symbol, assetType, refresh, cancellationToken);
            
            if (price == null)
                return NotFound($"Price data not found for {symbol}");
                
            return Ok(price);
        }

        [HttpGet("prices")]
        public async Task<ActionResult<IEnumerable<AssetPrice>>> GetPrices(
            [FromQuery] string[] symbols,
            [FromQuery] AssetType? assetType = null,
            [FromQuery] bool refresh = false,
            CancellationToken cancellationToken = default)
        {
            if (symbols == null || symbols.Length == 0)
                return BadRequest("At least one symbol must be provided");
                
            _logger.LogInformation("Getting prices for {Count} symbols", symbols.Length);
            
            var prices = await _marketDataService.GetAssetPricesAsync(symbols, assetType, refresh, cancellationToken);
            return Ok(prices);
        }

        [HttpGet("detail/{symbol}")]
        public async Task<ActionResult<AssetDetail>> GetDetail(
            string symbol,
            [FromQuery] AssetType assetType = AssetType.Stock,
            [FromQuery] bool refresh = false,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting details for {Symbol} ({AssetType})", symbol, assetType);
            
            var detail = await _marketDataService.GetAssetDetailAsync(symbol, assetType, refresh, cancellationToken);
            
            if (detail == null)
                return NotFound($"Detail data not found for {symbol}");
                
            return Ok(detail);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<IDictionary<string, bool>>> RefreshCache(
            [FromBody] RefreshRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request?.Symbols == null || request.Symbols.Length == 0)
                return BadRequest("At least one symbol must be provided");
                
            _logger.LogInformation("Refreshing cache for {Count} symbols", request.Symbols.Length);
            
            var results = await _marketDataService.RefreshAssetsCacheAsync(
                request.Symbols, request.AssetType, cancellationToken);
                
            return Ok(results);
        }
    }

    public class RefreshRequest
    {
        public string[] Symbols { get; set; }
        public AssetType? AssetType { get; set; }
    }
}