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

        public void Initialise()
        {

        }

        public virtual double SellReverse(double volume)
        {
            throw new NotImplementedException();
        }

        public virtual double SellStraight(double volume)
        {
            throw new NotImplementedException();
        }

        public async Task<double> BuyStraight(double volume)
        {
            var order = new MarketOrderRequest
            {
                Asset = "BTC",
                AssetPairId = "BTCUSD",
                OrderAction = MatchingEngine.Connector.Abstractions.Models.OrderAction.Buy,
                Volume = volume
            };

            var assetPair = await _assetServiceDecorator.GetEnabledAssetPairAsync(order.AssetPairId);


            var baseAsset = await _assetServiceDecorator.GetEnabledAssetAsync(assetPair.BaseAssetId);
            var quotingAsset = await _assetServiceDecorator.GetEnabledAssetAsync(assetPair.QuotingAssetId);

            var straight = order.Asset == baseAsset.Id || order.Asset == baseAsset.Name;
            var orderVolume = order.Volume.TruncateDecimalPlaces(straight ? baseAsset.Accuracy : quotingAsset.Accuracy);
            var minVolume = straight ? assetPair.MinVolume : assetPair.MinInvertedVolume;

            //TODO
            var clientId = "";
            var response = await _matchingEngineAdapter.HandleMarketOrderAsync(
                clientId: clientId,
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
