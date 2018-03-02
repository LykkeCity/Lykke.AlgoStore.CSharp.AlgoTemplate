using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using System;
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

        private string _assetPairId;
        private string _asset;
        private string _walletId;
    
        public TradingService(IMatchingEngineAdapter matchingEngineAdapter,
            IAssetServiceDecorator assetServiceDecorator,
            IAlgoSettingsService algoSettingsService)
        {
            _matchingEngineAdapter = matchingEngineAdapter;
            _assetServiceDecorator = assetServiceDecorator;
            _algoSettingsService = algoSettingsService;
        }

        public void Initialize()
        {
            _assetPairId = _algoSettingsService.GetAlgoInstanceAssetPair();
            _asset = _algoSettingsService.GetAlgoInstanceTradedAsset();
            _walletId = _algoSettingsService.GetAlgoInstanceWalletId();
        }

        public async Task<double> SellStraight(double volume)
        {
            var order = new MarketOrderRequest
            {
                Asset = _asset,
                AssetPairId = _assetPairId,
                OrderAction = MatchingEngine.Connector.Abstractions.Models.OrderAction.Sell,
                Volume = volume
            };

            var assetPair = await _assetServiceDecorator.GetEnabledAssetPairAsync(order.AssetPairId);
            //Validata asset pair if needed

            var baseAsset = await _assetServiceDecorator.GetEnabledAssetAsync(assetPair.BaseAssetId);
            var quotingAsset = await _assetServiceDecorator.GetEnabledAssetAsync(assetPair.QuotingAssetId);
            //Validate Asset if needed

            var straight = order.Asset == baseAsset.Id || order.Asset == baseAsset.Name;
            var orderVolume = order.Volume.TruncateDecimalPlaces(straight ? baseAsset.Accuracy : quotingAsset.Accuracy);
            var minVolume = straight ? assetPair.MinVolume : assetPair.MinInvertedVolume;
            //Validate volume if needed


            var response = await _matchingEngineAdapter.HandleMarketOrderAsync(
                clientId: _walletId,
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
                Asset = _asset,
                AssetPairId = _assetPairId,
                OrderAction = MatchingEngine.Connector.Abstractions.Models.OrderAction.Buy,
                Volume = volume
            };

            var assetPair = await _assetServiceDecorator.GetEnabledAssetPairAsync(order.AssetPairId);
            //Validata asset pair if needed

            var baseAsset = await _assetServiceDecorator.GetEnabledAssetAsync(assetPair.BaseAssetId);
            var quotingAsset = await _assetServiceDecorator.GetEnabledAssetAsync(assetPair.QuotingAssetId);
            //Validate Asset if needed

            var straight = order.Asset == baseAsset.Id || order.Asset == baseAsset.Name;
            var orderVolume = order.Volume.TruncateDecimalPlaces(straight ? baseAsset.Accuracy : quotingAsset.Accuracy);
            var minVolume = straight ? assetPair.MinVolume : assetPair.MinInvertedVolume;
            //Validate volume if needed

            var response = await _matchingEngineAdapter.HandleMarketOrderAsync(
                clientId: _walletId,
                assetPairId: order.AssetPairId,
                orderAction: order.OrderAction,
                volume: orderVolume,
                straight: straight,
                reservedLimitVolume: null);

            return response.Result;
        }
    }
}
