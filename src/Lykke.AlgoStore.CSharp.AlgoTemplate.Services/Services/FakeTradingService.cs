using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Strings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;
using System;
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
        private readonly ICurrentDataProvider _currentDataProvider;

        public FakeTradingService(IAlgoSettingsService algoSettingsService,
            IAlgoInstanceTradeRepository algoInstanceTradeRepository,
            IStatisticsRepository statisticsRepository,
            ICurrentDataProvider currentDataProvider)
        {
            _algoSettingsService = algoSettingsService;
            _algoInstanceTradeRepository = algoInstanceTradeRepository;
            _statisticsRepository = statisticsRepository;
            _currentDataProvider = currentDataProvider;
        }

        public async Task Initialize(string instanceId, string assetPairId, bool straight)
        {
            _instanceId = instanceId;
            _assetPairId = assetPairId;
            _straight = straight;
            _tradedAssetId = _algoSettingsService.GetTradedAssetId();
            _oppositeAssetId = _algoSettingsService.GetAlgoInstanceOppositeAssetId();
        }

        /// <summary>
        /// Make a fake trade "Sell"
        /// </summary>
        /// <param name="tradeRequest">Trade request</param>
        public async Task<ResponseModel<double>> Sell(ITradeRequest tradeRequest)
        {
            double tradedOppositeVolume = CalculateOppositeOfTradedAssetTradeValue(tradeRequest.Volume, tradeRequest.Price);

            var summary = await _statisticsRepository.GetSummaryAsync(_instanceId);

            if (summary.LastTradedAssetBalance < tradeRequest.Volume)
                return new ResponseModel<double>()
                {
                    Error = new ResponseModel.ErrorModel()
                    {
                        Message = ErrorMessages.NotEnoughFunds,
                        Code = ResponseModel.ErrorCodeType.NotEnoughFunds
                    }
                };

            await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                CreateAlgoInstanceTrade(_tradedAssetId, -tradeRequest.Volume, false));

            await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                CreateAlgoInstanceTrade(_oppositeAssetId, tradedOppositeVolume, false));

            summary.LastTradedAssetBalance -= tradeRequest.Volume;
            summary.LastAssetTwoBalance += tradedOppositeVolume;

            if (_straight)
                summary.LastWalletBalance = Math.Round(summary.LastAssetTwoBalance + summary.LastTradedAssetBalance * tradeRequest.Price, 8);
            else
                summary.LastWalletBalance = Math.Round(summary.LastTradedAssetBalance + summary.LastAssetTwoBalance * tradeRequest.Price, 8);

            summary.TotalNumberOfTrades++;

            await _statisticsRepository.CreateOrUpdateSummaryAsync(summary);

            return new ResponseModel<double>()
            {
                Result = tradeRequest.Price
            };
        }

        /// <summary>
        /// Make a fake trade "Buy"
        /// </summary>
        /// <param name="tradeRequest">Trade request</param>
        public async Task<ResponseModel<double>> Buy(ITradeRequest tradeRequest)
        {
            double tradedOppositeValue = CalculateOppositeOfTradedAssetTradeValue(tradeRequest.Volume, tradeRequest.Price);

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
                    CreateAlgoInstanceTrade(_tradedAssetId, tradeRequest.Volume, true));

            await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                CreateAlgoInstanceTrade(_oppositeAssetId, -tradedOppositeValue, true));

            summary.LastTradedAssetBalance += tradeRequest.Volume;
            summary.LastAssetTwoBalance -= tradedOppositeValue;

            if (_straight)
                summary.LastWalletBalance = Math.Round(summary.LastAssetTwoBalance + summary.LastTradedAssetBalance * tradeRequest.Price, 8);
            else
                summary.LastWalletBalance = Math.Round(summary.LastTradedAssetBalance + summary.LastAssetTwoBalance * tradeRequest.Price, 8);

            summary.TotalNumberOfTrades++;

            await _statisticsRepository.CreateOrUpdateSummaryAsync(summary);

            return new ResponseModel<double>()
            {
                Result = tradeRequest.Price
            };
        }

        private AlgoInstanceTrade CreateAlgoInstanceTrade(string tradeAsset, double amount, bool isBuy)
        {
            return new AlgoInstanceTrade()
            {
                InstanceId = _instanceId,
                AssetPairId = _assetPairId,
                AssetId = tradeAsset,
                Amount = amount,
                Price = _currentDataProvider.CurrentPrice,
                DateOfTrade =_currentDataProvider.CurrentTimestamp,
                IsBuy = isBuy
            };
        }

        private double CalculateOppositeOfTradedAssetTradeValue(double volume, double closePrice)
        {
            double tradedOppositeValue;
            if (_straight)
                tradedOppositeValue = volume * closePrice;
            else
                tradedOppositeValue = volume / closePrice;

            return Math.Round(tradedOppositeValue, 8);
        }
    }
}
