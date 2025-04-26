using System;

namespace MarketData.Application.Services;

public class SymbolValidator
{
    public IReadOnlyCollection<string> NormalizeAndValidateSymbols(IEnumerable<string> symbols)
    {
        if (symbols == null)
            throw new ArgumentNullException(nameof(symbols));
            
        var result = new List<string>();
        var uniqueSymbols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        
        foreach (var symbol in symbols)
        {
            ValidateSymbol(symbol);
            var normalizedSymbol = NormalizeSymbol(symbol);
                
            if (uniqueSymbols.Add(normalizedSymbol))
            {
                result.Add(normalizedSymbol);
            }
        }
    
        return result;
    }

    public string NormalizeAndValidateSymbol(string symbol)
    {
        ValidateSymbol(symbol);
        return NormalizeSymbol(symbol);
    }

    private void ValidateSymbol(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));
    }

    private string NormalizeSymbol(string symbol)
    {
        return symbol.ToUpperInvariant().Trim();
    }
}
