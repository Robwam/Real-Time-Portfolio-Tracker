using System.Text.Json.Serialization;

namespace Shared.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AssetType
{
    Stock,
    Crypto,
    Other
} // To do: Add more asset types: ETF, Bond, Commodity, Cash, etc.