using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Strings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Core.Domain;

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

        /// <summary>
        /// Make a fake trade "Sell"
        /// </summary>
        /// <param name="volume">Volume that we want to trade</param>
        /// <param name="candleData">Candle data that is taken from history service</param>
        public async Task<ResponseModel<double>> Sell(double volume, IAlgoCandle candleData)
        {
            double tradedOppositeVolume = CalculateOppositeOfTradedAssetTradeValue(volume, candleData.Close);

            var summary = await _statisticsRepository.GetSummaryAsync(_instanceId);

            if (summary.LastTradedAssetBalance < volume)
                return new ResponseModel<double>()
                {
                    Error = new ResponseModel.ErrorModel()
                    {
                        Message = ErrorMessages.NotEnoughFunds,
                        Code = ResponseModel.ErrorCodeType.NotEnoughFunds
                    }
                };

            await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                CreateAlgoInstanceTrade(_tradedAssetId, -volume, candleData, false));

            await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                CreateAlgoInstanceTrade(_oppositeAssetId, tradedOppositeVolume, candleData, false));

            summary.LastTradedAssetBalance -= volume;
            summary.LastAssetTwoBalance += tradedOppositeVolume;

            if (_straight)
                summary.LastWalletBalance = Math.Round(summary.LastAssetTwoBalance + summary.LastTradedAssetBalance * candleData.Close, 8);
            else
                summary.LastWalletBalance = Math.Round(summary.LastTradedAssetBalance + summary.LastAssetTwoBalance * candleData.Close, 8);

            await _statisticsRepository.CreateOrUpdateSummaryAsync(summary);

            return new ResponseModel<double>()
            {
                Result = candleData.Close
            };
        }

        /// <summary>
        /// Make a fake trade "Buy"
        /// </summary>
        /// <param name="volume">Volume that we want to trade</param>
        /// <param name="candleData">Candle data that is taken from history service</param>
        public async Task<ResponseModel<double>> Buy(double volume, IAlgoCandle candleData)
        {
            double tradedOppositeValue = CalculateOppositeOfTradedAssetTradeValue(volume, candleData.Close);

            var summary = await _statisticsRepository.GetSummaryAsync(_instanceId);

            if (summary.LastAssetTwoBalance < tradedOppositeValue)
                return new ResponseModel<double>()
                {
                    Error = new ResponseModel.ErrorModel()
                    {
                        Message = ErrorMessages.NotEnoughFunds,
                        Code = ResponseModel.ErrorCodeType.NotEnoughFunds
                    }
                };

            await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                    CreateAlgoInstanceTrade(_tradedAssetId, volume, candleData, true));

            await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                CreateAlgoInstanceTrade(_oppositeAssetId, -tradedOppositeValue, candleData, true));

            summary.LastTradedAssetBalance += volume;
            summary.LastAssetTwoBalance -= tradedOppositeValue;

            if (_straight)
                summary.LastWalletBalance = Math.Round(summary.LastAssetTwoBalance + summary.LastTradedAssetBalance * candleData.Close, 8);
            else
                summary.LastWalletBalance = Math.Round(summary.LastTradedAssetBalance + summary.LastAssetTwoBalance * candleData.Close, 8);

            await _statisticsRepository.CreateOrUpdateSummaryAsync(summary);

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

        private double CalculateOppositeOfTradedAssetTradeValue(double volume, double closePrice)
        {
            double tradedOppositeValue = 0;
            if (_straight)
                tradedOppositeValue = volume * closePrice;
            else
                tradedOppositeValue = volume / closePrice;

            return Math.Round(tradedOppositeValue, 8);
        }
    }
}
