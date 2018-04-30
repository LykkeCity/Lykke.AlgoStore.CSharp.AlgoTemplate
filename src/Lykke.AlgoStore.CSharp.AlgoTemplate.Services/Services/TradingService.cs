using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using System;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;

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
        private readonly IAlgoSettingsService _algoSettingsService;

        private string _assetPairId;
        private string _asset;
        private string _walletId;
        private bool _straight;

        public TradingService(IMatchingEngineAdapter matchingEngineAdapter,
            IAlgoSettingsService algoSettingsService)
        {
            _matchingEngineAdapter = matchingEngineAdapter;
            _algoSettingsService = algoSettingsService;
        }

        public void Initialize()
        {
            _assetPairId = _algoSettingsService.GetAlgoInstanceAssetPair();
            _asset = _algoSettingsService.GetTradedAsset();
            _walletId = _algoSettingsService.GetAlgoInstanceWalletId();
            _straight = _algoSettingsService.IsAlgoInstanceMarketOrderStraight();
        }

        public async Task<ResponseModel<double>> Sell(double volume)
        {
            var order = new MarketOrderRequest
            {
                Asset = _asset,
                AssetPairId = _assetPairId,
                OrderAction = MatchingEngine.Connector.Abstractions.Models.OrderAction.Sell,
                Volume = volume
            };

            var response = await _matchingEngineAdapter.HandleMarketOrderAsync(
                clientId: _walletId,
                assetPairId: order.AssetPairId,
                orderAction: order.OrderAction,
                volume: volume,
                straight: _straight,
                reservedLimitVolume: null);

            return response;
        }

        public async Task<ResponseModel<double>> Buy(double volume)
        {
            var order = new MarketOrderRequest
            {
                Asset = _asset,
                AssetPairId = _assetPairId,
                OrderAction = MatchingEngine.Connector.Abstractions.Models.OrderAction.Buy,
                Volume = volume
            };

            var response = await _matchingEngineAdapter.HandleMarketOrderAsync(
                clientId: _walletId,
                assetPairId: order.AssetPairId,
                orderAction: order.OrderAction,
                volume: volume,
                straight: _straight,
                reservedLimitVolume: null);

            return response;
        }
    }
}
