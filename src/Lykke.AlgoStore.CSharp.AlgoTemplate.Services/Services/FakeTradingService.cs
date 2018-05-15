using System;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class FakeTradingService : IFakeTradingService
    {
        private string _assetPairId;
        private string _tradedAssetId;
        private string _oppositeAssetId;
        private string _instanceId;
        private bool _straight;

        private readonly IAlgoSettingsService _algoSettingsService;
        private readonly IAlgoInstanceTradeRepository _algoInstanceTradeRepository;
        private readonly IStatisticsRepository _statisticsRepository;

        public FakeTradingService(IAlgoSettingsService algoSettingsService,
            IAlgoInstanceTradeRepository algoInstanceTradeRepository,
            IStatisticsRepository statisticsRepository)
        {
            _algoSettingsService = algoSettingsService;
            _algoInstanceTradeRepository = algoInstanceTradeRepository;
            _statisticsRepository = statisticsRepository;
        }

        public void Initialize(string instanceId, string assetPairId, bool straight)
        {
            _instanceId = instanceId;
            _assetPairId = assetPairId;
            _straight = straight;
            _tradedAssetId = _algoSettingsService.GetTradedAsset();
            _oppositeAssetId = _algoSettingsService.GetAlgoInstanceOppositeAssetId();
        }

        public async Task<ResponseModel<double>> Sell(double volume, IAlgoCandle candleData)
        {
            //save traded asset trade
            await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                CreateAlgoInstanceTrade(_tradedAssetId, -volume, candleData, false));

            double tradedOpositeVolume = CalculateOpositeOfTradedAssetTradeValue(volume, candleData.Close);

            //save oposite asset trade of traded asset
            await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                CreateAlgoInstanceTrade(_oppositeAssetId, tradedOpositeVolume, candleData, false));


            var summary = await _statisticsRepository.GetSummaryAsync(_instanceId);
            summary.LastTradedAssetBalance -= volume;
            summary.LastAssetTwoBalance += tradedOpositeVolume;

            return new ResponseModel<double>()
            {
                Result = candleData.Close
            };
        }

        public async Task<ResponseModel<double>> Buy(double volume, IAlgoCandle candleData)
        {
            double tradedOpositeVolume = CalculateOpositeOfTradedAssetTradeValue(volume, candleData.Close);

            //save traded asset trade

            await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                CreateAlgoInstanceTrade(_tradedAssetId, volume, candleData, true));

            //save oposite asset trade of traded asset
            await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                CreateAlgoInstanceTrade(_oppositeAssetId, -tradedOpositeVolume, candleData, true));

            var summary = await _statisticsRepository.GetSummaryAsync(_instanceId);
            summary.LastTradedAssetBalance += volume;
            summary.LastAssetTwoBalance -= tradedOpositeVolume;

            return new ResponseModel<double>()
            {
                Result = candleData.Close
            };
        }

        private AlgoInstanceTrade CreateAlgoInstanceTrade(string tradeAsset, double amount, IAlgoCandle candleData, bool isBuy)
        {
            return new AlgoInstanceTrade()
            {
                InstanceId = _instanceId,
                AssetPairId = _assetPairId,
                AssetId = tradeAsset,
                Amount = amount,
                Price = candleData.Close,
                DateOfTrade = candleData.DateTime,
                IsBuy = isBuy
            };
        }

        private double CalculateOpositeOfTradedAssetTradeValue(double volume, double closePrice)
        {
            double tradedOpositeVolume = 0;
            if (_straight)
                tradedOpositeVolume = volume * closePrice;
            else
                tradedOpositeVolume = volume / closePrice;

            return Math.Round(tradedOpositeVolume, 8);
        }
    }
}
