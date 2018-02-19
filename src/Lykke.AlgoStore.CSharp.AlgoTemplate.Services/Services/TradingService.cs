using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="ITradingService"/> implementation
    /// </summary>
    public class TradingService : ITradingService
    {
        /// <summary>
        /// This class is placeholder for sample exception and 
        /// should be renamed/removed
        /// </summary>
        public class TradingServiceException : Exception { }

        private readonly IMatchingEngineAdapter _matchingEngineAdapter;
        private readonly IAssetServiceDecorator _assetServiceDecorator;
        private readonly IAlgoSettingsService _algoSettingsService;

        private string AssetPairId;
        private string Asset;
        private string ClientId;
    
        public TradingService(IMatchingEngineAdapter matchingEngineAdapter,
            IAssetServiceDecorator assetServiceDecorator,
            IAlgoSettingsService algoSettingsService)
        {
            _matchingEngineAdapter = matchingEngineAdapter;
            _assetServiceDecorator = assetServiceDecorator;
            _algoSettingsService = algoSettingsService;
        }


        public void Initialise()
        {
            AssetPairId = _algoSettingsService.GetSetting("AssetPairId");
            Asset = _algoSettingsService.GetSetting("Asset");
            ClientId = _algoSettingsService.GetSetting("ClientId");
        }

        public virtual double SellReverse(double volume)
        {
            throw new NotImplementedException();
        }

        public async Task<double> SellStraight(double volume)
        {
            var order = new MarketOrderRequest
            {
                Asset = Asset,
                AssetPairId = AssetPairId,
                OrderAction = MatchingEngine.Connector.Abstractions.Models.OrderAction.Sell,
                Volume = volume
            };

            var assetPair = await _assetServiceDecorator.GetEnabledAssetPairAsync(order.AssetPairId);

            var baseAsset = await _assetServiceDecorator.GetEnabledAssetAsync(assetPair.BaseAssetId);
            var quotingAsset = await _assetServiceDecorator.GetEnabledAssetAsync(assetPair.QuotingAssetId);

            var straight = order.Asset == baseAsset.Id || order.Asset == baseAsset.Name;
            var orderVolume = order.Volume.TruncateDecimalPlaces(straight ? baseAsset.Accuracy : quotingAsset.Accuracy);
            var minVolume = straight ? assetPair.MinVolume : assetPair.MinInvertedVolume;

            var response = await _matchingEngineAdapter.HandleMarketOrderAsync(
                clientId: ClientId,
                assetPairId: order.AssetPairId,
                orderAction: order.OrderAction,
                volume: orderVolume,
                straight: straight,
                reservedLimitVolume: null);

            return response.Result;
        }

        public async Task<double> BuyStraight(double volume)
        {
            var order = new MarketOrderRequest
            {
                Asset = Asset,
                AssetPairId = AssetPairId,
                OrderAction = MatchingEngine.Connector.Abstractions.Models.OrderAction.Buy,
                Volume = volume
            };

            var assetPair = await _assetServiceDecorator.GetEnabledAssetPairAsync(order.AssetPairId);

            var baseAsset = await _assetServiceDecorator.GetEnabledAssetAsync(assetPair.BaseAssetId);
            var quotingAsset = await _assetServiceDecorator.GetEnabledAssetAsync(assetPair.QuotingAssetId);

            var straight = order.Asset == baseAsset.Id || order.Asset == baseAsset.Name;
            var orderVolume = order.Volume.TruncateDecimalPlaces(straight ? baseAsset.Accuracy : quotingAsset.Accuracy);
            var minVolume = straight ? assetPair.MinVolume : assetPair.MinInvertedVolume;
           
            var response = await _matchingEngineAdapter.HandleMarketOrderAsync(
                clientId: ClientId,
                assetPairId: order.AssetPairId,
                orderAction: order.OrderAction,
                volume: orderVolume,
                straight: straight,
                reservedLimitVolume: null);

            return response.Result;
        }

        public double BuyReverse(double volume)
        {
            throw new NotImplementedException();
        }
    }
}
