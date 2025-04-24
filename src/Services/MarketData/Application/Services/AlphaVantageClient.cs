using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MarketData.Application.Interfaces;
using MarketData.Models;
using Microsoft.Extensions.Logging;
using Shared.Models.Enums;

namespace MarketData.Infrastructure.ExternalClients;
public class AlphaVantageClient : IExternalClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly AssetType _assetType;
    private readonly ILogger _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public AlphaVantageClient(
        HttpClient httpClient,
        string apiKey,
        AssetType assetType,
        ILogger logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _assetType = assetType;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public bool SupportsBatchRequests => false; // Alpha Vantage free tier doesn't support batch requests

    public async Task<AssetPrice> GetAssetPriceAsync(string symbol, CancellationToken cancellationToken = default)
    {
        try
        {
            // Build appropriate endpoint based on asset type
            string endpoint = _assetType switch
            {
                AssetType.Stock => $"query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={_apiKey}",
                AssetType.Crypto => $"query?function=CURRENCY_EXCHANGE_RATE&from_currency={symbol}&to_currency=USD&apikey={_apiKey}",
                _ => throw new NotSupportedException($"Asset type {_assetType} is not supported")
            };
            
            // Send request
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            
            // Parse response based on asset type
            return _assetType switch
            {
                AssetType.Stock => ParseStockResponse(content, symbol),
                AssetType.Crypto => ParseCryptoResponse(content, symbol),
                _ => null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching price from Alpha Vantage for {Symbol}", symbol);
            return null;
        }
    }

    public async Task<IEnumerable<AssetPrice>> GetAssetPricesAsync(IEnumerable<string> symbols, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Alpha Vantage does not support batch requests");
    }

    public async Task<AssetDetail> GetAssetDetailAsync(string symbol, CancellationToken cancellationToken = default)
    {
        try
        {
            // Build appropriate endpoint based on asset type
            string endpoint = _assetType switch
            {
                AssetType.Stock => $"query?function=OVERVIEW&symbol={symbol}&apikey={_apiKey}",
                AssetType.Crypto => $"query?function=DIGITAL_CURRENCY_DAILY&symbol={symbol}&market=USD&apikey={_apiKey}",
                _ => throw new NotSupportedException($"Asset type {_assetType} is not supported")
            };
            
            // Send request
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            
            // Parse response based on asset type
            return _assetType switch
            {
                AssetType.Stock => ParseStockDetailResponse(content, symbol),
                AssetType.Crypto => ParseCryptoDetailResponse(content, symbol),
                _ => null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching details from Alpha Vantage for {Symbol}", symbol);
            return null;
        }
    }

    #region Response Parsing

    private AssetPrice ParseStockResponse(string content, string symbol)
    {
        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;
            
            // Check if we have a "Global Quote" element in the response
            if (!root.TryGetProperty("Global Quote", out var quoteElement))
            {
                _logger.LogWarning("Unexpected Alpha Vantage response format for {Symbol}", symbol);
                return null;
            }
            
            // Extract required data
            quoteElement.TryGetProperty("05. price", out var priceElement);
            quoteElement.TryGetProperty("02. open", out var openElement);
            quoteElement.TryGetProperty("03. high", out var highElement);
            quoteElement.TryGetProperty("04. low", out var lowElement);
            quoteElement.TryGetProperty("08. previous close", out var prevCloseElement);
            quoteElement.TryGetProperty("06. volume", out var volumeElement);
            quoteElement.TryGetProperty("07. latest trading day", out var dateElement);
            
            if (!decimal.TryParse(priceElement.GetString(), out var price))
            {
                _logger.LogWarning("Failed to parse price for {Symbol}", symbol);
                return null;
            }
            
            // Create and populate AssetPrice object
            var result = new AssetPrice
            {
                Symbol = symbol,
                Price = price,
                LastUpdated = DateTime.Now
            };
            
            // Add optional fields if available
            if (decimal.TryParse(openElement.GetString(), out var open))
                result.Open = open;
                
            if (decimal.TryParse(highElement.GetString(), out var high))
                result.High = high;
                
            if (decimal.TryParse(lowElement.GetString(), out var low))
                result.Low = low;
                
            if (decimal.TryParse(prevCloseElement.GetString(), out var prevClose))
                result.PreviousClose = prevClose;
                
            if (long.TryParse(volumeElement.GetString(), out var volume))
                result.Volume = volume;
                
            if (DateTime.TryParse(dateElement.GetString(), out var date))
                result.LastUpdated = date;
                
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing Alpha Vantage stock response for {Symbol}", symbol);
            return null;
        }
    }

    private AssetPrice ParseCryptoResponse(string content, string symbol)
    {
        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;
            
            // Check if we have the expected element in the response
            if (!root.TryGetProperty("Realtime Currency Exchange Rate", out var rateElement))
            {
                _logger.LogWarning("Unexpected Alpha Vantage response format for crypto {Symbol}", symbol);
                return null;
            }
            
            // Extract required data
            rateElement.TryGetProperty("5. Exchange Rate", out var rateValueElement);
            rateElement.TryGetProperty("6. Last Refreshed", out var refreshedElement);
            
            if (!decimal.TryParse(rateValueElement.GetString(), out var price))
            {
                _logger.LogWarning("Failed to parse price for crypto {Symbol}", symbol);
                return null;
            }
            
            // Create and populate AssetPrice object
            var result = new AssetPrice
            {
                Symbol = symbol,
                Price = price,
                LastUpdated = DateTime.Now
            };
            
            // Add last updated time if available
            if (DateTime.TryParse(refreshedElement.GetString(), out var refreshed))
                result.LastUpdated = refreshed;
                
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing Alpha Vantage crypto response for {Symbol}", symbol);
            return null;
        }
    }

    private AssetDetail ParseStockDetailResponse(string content, string symbol)
    {
        try
        {
            // For stock overview, the response is a flat JSON object with company details
            var companyData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(content, _jsonOptions);
            
            if (companyData == null || companyData.Count == 0 || !companyData.ContainsKey("Symbol"))
            {
                _logger.LogWarning("Empty or invalid Alpha Vantage stock detail response for {Symbol}", symbol);
                return null;
            }
            
            // Extract key fields
            companyData.TryGetValue("Name", out var nameElement);
            companyData.TryGetValue("Description", out var descriptionElement);
            companyData.TryGetValue("Sector", out var sectorElement);
            companyData.TryGetValue("Industry", out var industryElement);
            companyData.TryGetValue("MarketCapitalization", out var marketCapElement);
            companyData.TryGetValue("PERatio", out var peRatioElement);
            companyData.TryGetValue("DividendYield", out var dividendYieldElement);
            companyData.TryGetValue("52WeekHigh", out var yearHighElement);
            companyData.TryGetValue("52WeekLow", out var yearLowElement);
            
            // Create asset detail object
            var detail = new AssetDetail
            {
                Symbol = symbol,
                Name = nameElement.ValueKind != JsonValueKind.Undefined ? nameElement.GetString() : null,
                Description = descriptionElement.ValueKind != JsonValueKind.Undefined ? descriptionElement.GetString() : null,
                AssetType = AssetType.Stock,
                Category = sectorElement.ValueKind != JsonValueKind.Undefined ? sectorElement.GetString() : null,
                Subcategory = industryElement.ValueKind != JsonValueKind.Undefined ? industryElement.GetString() : null,
                LastUpdated = DateTime.Now
            };
            
            // Add numeric fields if available
            if (marketCapElement.ValueKind != JsonValueKind.Undefined && decimal.TryParse(marketCapElement.GetString(), out var marketCap))
                detail.MarketCap = marketCap;
                
            if (peRatioElement.ValueKind != JsonValueKind.Undefined && decimal.TryParse(peRatioElement.GetString(), out var peRatio))
                detail.PeRatio = peRatio;
                
            if (dividendYieldElement.ValueKind != JsonValueKind.Undefined && decimal.TryParse(dividendYieldElement.GetString(), out var dividendYield))
                detail.DividendYield = dividendYield;
                
            if (yearHighElement.ValueKind != JsonValueKind.Undefined && decimal.TryParse(yearHighElement.GetString(), out var yearHigh))
                detail.YearHigh = yearHigh;
                
            if (yearLowElement.ValueKind != JsonValueKind.Undefined && decimal.TryParse(yearLowElement.GetString(), out var yearLow))
                detail.YearLow = yearLow;
            
            return detail;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing Alpha Vantage stock detail response for {Symbol}", symbol);
            return null;
        }
    }

    private AssetDetail ParseCryptoDetailResponse(string content, string symbol)
    {
        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;
            
            // Check if we have the expected elements
            if (!root.TryGetProperty("Meta Data", out var metaData))
            {
                _logger.LogWarning("Unexpected Alpha Vantage response format for crypto detail {Symbol}", symbol);
                return null;
            }
            
            // Extract metadata
            metaData.TryGetProperty("1. Information", out var infoElement);
            metaData.TryGetProperty("2. Digital Currency Code", out var codeElement);
            metaData.TryGetProperty("3. Digital Currency Name", out var nameElement);
            metaData.TryGetProperty("6. Last Refreshed", out var refreshedElement);
            
            // Create asset detail
            var detail = new AssetDetail
            {
                Symbol = symbol,
                Name = nameElement.ValueKind != JsonValueKind.Undefined ? nameElement.GetString() : symbol,
                Description = infoElement.ValueKind != JsonValueKind.Undefined ? infoElement.GetString() : null,
                AssetType = AssetType.Crypto,
                Category = "Cryptocurrency",
                LastUpdated = DateTime.Now
            };
            
            // Add last updated if available
            if (refreshedElement.ValueKind != JsonValueKind.Undefined && DateTime.TryParse(refreshedElement.GetString(), out var refreshed))
                detail.LastUpdated = refreshed;
            
            // Try to extract additional data from time series if available
            if (root.TryGetProperty("Time Series (Digital Currency Daily)", out var timeSeries))
            {
                // Get first entry (most recent)
                using var timeSeriesEnum = timeSeries.EnumerateObject();
                if (timeSeriesEnum.MoveNext())
                {
                    var firstEntry = timeSeriesEnum.Current.Value;
                    
                    // Parse high/low values
                    if (firstEntry.TryGetProperty("2a. high (USD)", out var highElement) && 
                        decimal.TryParse(highElement.GetString(), out var high))
                        detail.YearHigh = high;
                        
                    if (firstEntry.TryGetProperty("3a. low (USD)", out var lowElement) && 
                        decimal.TryParse(lowElement.GetString(), out var low))
                        detail.YearLow = low;
                }
            }
            
            return detail;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing Alpha Vantage crypto detail response for {Symbol}", symbol);
            return null;
        }
    }
    
    #endregion
}