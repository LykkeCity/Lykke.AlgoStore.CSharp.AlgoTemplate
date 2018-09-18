using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using System;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class FakeLimitOrdersHandler : IFakeLimitOrdersHandler
    {
        private const string FakeWalletStatic = "FakeTradingWallet";

        private string _tradedAssetId;
        private string _oppositeAssetId;
        private string _instanceId;
        private string _assetPairId;
        private bool _straight;
        private StatisticsSummary _summary;

        private readonly IAlgoSettingsService _algoSettingsService;
        private readonly IAlgoInstanceTradeRepository _algoInstanceTradeRepository;
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly ILimitOrderManager _limitOrderManager;
        private readonly object _sync = new object();

        public FakeLimitOrdersHandler(IAlgoSettingsService algoSettingsService, 
            IAlgoInstanceTradeRepository algoInstanceTradeRepository,
            IStatisticsRepository statisticsRepository,
            ILimitOrderManager limitOrderManager)
        {
            _algoSettingsService = algoSettingsService;
            _algoInstanceTradeRepository = algoInstanceTradeRepository;
            _statisticsRepository = statisticsRepository;
            _limitOrderManager = limitOrderManager;
        }

        public async Task Initialize()
        {
            _instanceId = _algoSettingsService.GetInstanceId();
            _assetPairId = _algoSettingsService.GetAlgoInstanceAssetPairId();
            _straight = _algoSettingsService.IsAlgoInstanceMarketOrderStraight();
            _tradedAssetId = _algoSettingsService.GetTradedAssetId();
            _oppositeAssetId = _algoSettingsService.GetAlgoInstanceOppositeAssetId();
            _summary = await _statisticsRepository.GetSummaryAsync(_instanceId);
        }

        public async Task HandleLimitOrders(Candle currentCandle)
        {
            lock (_sync)
            {
                foreach (var limitOrder in _limitOrderManager)
                {
                    if (limitOrder.Status != OrderStatus.Matched)
                    {                 
                        if ((limitOrder.Action == OrderAction.Buy && currentCandle.Low <= limitOrder.Price) || 
                            (limitOrder.Action == OrderAction.Sell && currentCandle.High >= limitOrder.Price))
                        {
                            FulfillLimitOrder(limitOrder, currentCandle.Close, currentCandle.DateTime);
                            limitOrder.MarkMatched();
                            MarkDbOrderMatched(limitOrder.Id);
                            _summary.TotalNumberOfTrades++;
                        }
                    }
                }
            }
        }

        public async Task HandleLimitOrders(IAlgoQuote currentQuote)
        {
            lock (_sync)
            {
                foreach (var limitOrder in _limitOrderManager)
                {
                    if (limitOrder.Status != OrderStatus.Matched)
                    {
                        if ((limitOrder.Action == OrderAction.Buy && currentQuote.Price <= limitOrder.Price) ||
                            (limitOrder.Action == OrderAction.Sell && currentQuote.Price >= limitOrder.Price))
                        {
                            FulfillLimitOrder(limitOrder, currentQuote.Price, currentQuote.Timestamp);
                            limitOrder.MarkMatched();
                            MarkDbOrderMatched(limitOrder.Id);
                            _summary.TotalNumberOfTrades++;
                        }
                    }
                }
            }
        }

        private async Task FulfillLimitOrder(ILimitOrder limitOrder, double fullfilmentPrice, DateTime dayOfFulfillment)
        {
            var oppositeAssetAmount = Math.Round(limitOrder.Volume * fullfilmentPrice, 8);
            var limitOrderId = limitOrder.Id.ToString();

            if (_straight)
            {
                if (limitOrder.Action == OrderAction.Buy)
                {
                    await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                        CreateAlgoInstanceLimitTrade(limitOrderId, _tradedAssetId, limitOrder.Volume, fullfilmentPrice, dayOfFulfillment, true));

                    await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                        CreateAlgoInstanceLimitTrade(limitOrderId, _oppositeAssetId, -oppositeAssetAmount, fullfilmentPrice, dayOfFulfillment, true));

                    _summary.LastTradedAssetBalance += limitOrder.Volume;
                    _summary.LastAssetTwoBalance -= oppositeAssetAmount;
                }
                else
                {
                    await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                        CreateAlgoInstanceLimitTrade(limitOrderId, _tradedAssetId, -limitOrder.Volume, fullfilmentPrice, dayOfFulfillment, false));

                    await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                        CreateAlgoInstanceLimitTrade(limitOrderId, _oppositeAssetId, oppositeAssetAmount, fullfilmentPrice, dayOfFulfillment, false));

                    _summary.LastTradedAssetBalance -= limitOrder.Volume;
                    _summary.LastAssetTwoBalance += oppositeAssetAmount;
                }
            }
            else
            {
                if (limitOrder.Action == OrderAction.Buy)
                {
                    await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                        CreateAlgoInstanceLimitTrade(limitOrderId, _oppositeAssetId, limitOrder.Volume, fullfilmentPrice, dayOfFulfillment, true));

                    await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                        CreateAlgoInstanceLimitTrade(limitOrderId, _tradedAssetId, -oppositeAssetAmount, fullfilmentPrice, dayOfFulfillment, true));

                    _summary.LastAssetTwoBalance += limitOrder.Volume;
                    _summary.LastTradedAssetBalance -= oppositeAssetAmount;
                }
                else
                {
                    await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                        CreateAlgoInstanceLimitTrade(limitOrderId, _oppositeAssetId, -limitOrder.Volume, fullfilmentPrice, dayOfFulfillment, true));

                    await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(
                        CreateAlgoInstanceLimitTrade(limitOrderId, _tradedAssetId, oppositeAssetAmount, fullfilmentPrice, dayOfFulfillment, true));

                    _summary.LastAssetTwoBalance -= limitOrder.Volume;
                    _summary.LastTradedAssetBalance += oppositeAssetAmount;
                }
            }

            if (_straight)
                _summary.LastWalletBalance = Math.Round(_summary.LastAssetTwoBalance + _summary.LastTradedAssetBalance * limitOrder.Price, 8);
            else
                _summary.LastWalletBalance = Math.Round(_summary.LastTradedAssetBalance + _summary.LastAssetTwoBalance * limitOrder.Price, 8);

            await _statisticsRepository.CreateOrUpdateSummaryAsync(_summary);
        }

        private async Task MarkDbOrderMatched(Guid orderId)
        {
            var limitOrderInDb = await _algoInstanceTradeRepository.GetAlgoInstanceOrderAsync(KeyGenerator.GenerateFakeLimitOrderPartitionKey(orderId),
                FakeWalletStatic);

            if (limitOrderInDb == null)
            {
                throw new InvalidOperationException($"Could not find AlgoInstanceOrder with id {orderId}");
            }

            limitOrderInDb.OrderStatus = OrderStatus.Matched;

            await _algoInstanceTradeRepository.CreateOrUpdateAlgoInstanceOrderAsync(limitOrderInDb);
        }

        private AlgoInstanceTrade CreateAlgoInstanceLimitTrade(string limitOrderId, string tradedAsset, double amount, double executingPrice, DateTime executingDate, bool isBuy)
        {
            return new AlgoInstanceTrade()
            {
                OrderId = limitOrderId,
                InstanceId = _instanceId,
                AssetPairId = _assetPairId,
                AssetId = tradedAsset,
                Amount = amount,
                Price = executingPrice,
                DateOfTrade = executingDate,
                IsBuy = isBuy
            };
        }
    }
}
