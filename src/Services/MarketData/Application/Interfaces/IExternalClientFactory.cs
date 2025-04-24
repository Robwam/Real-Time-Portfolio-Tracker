using Shared.Models.Enums;

namespace MarketData.Application.Interfaces;

public interface IExternalClientFactory
{
    IExternalClient GetClient(AssetType assetType);
}