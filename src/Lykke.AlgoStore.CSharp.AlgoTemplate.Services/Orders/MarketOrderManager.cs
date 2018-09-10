using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Charting;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Extensions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Orders
{
    public class MarketOrderManager : IMarketOrderManager
    {
        private readonly ITradingService _tradingService;
        private readonly IAlgoSettingsService _settingsService;
        private readonly IStatisticsService _statisticsService;
        private readonly IUserLogService _userLogService;
        private readonly IEventCollector _eventCollector;
        private readonly ICurrentDataProvider _currentDataProvider;

        private readonly List<MarketOrder> _marketOrders = new List<MarketOrder>();

        #region IReadOnlyList implementation

        public IMarketOrder this[int index] => _marketOrders[index];
        public int Count => _marketOrders.Count;

        public IEnumerator<IMarketOrder> GetEnumerator() => _marketOrders.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _marketOrders.GetEnumerator();

        #endregion // IReadOnlyList implementation

        public MarketOrderManager(
            ITradingService tradingService,
            IAlgoSettingsService settingsService,
            IStatisticsService statisticsService,
            IUserLogService userLogService,
            IEventCollector eventCollector,
            ICurrentDataProvider currentDataProvider)
        {
            _tradingService = tradingService;
            _settingsService = settingsService;
            _statisticsService = statisticsService;
            _userLogService = userLogService;
            _eventCollector = eventCollector;
            _currentDataProvider = currentDataProvider;
        }

        public IMarketOrder Create(Algo.OrderAction action, double volume)
        {
            if (volume <= 0)
                throw new ArgumentException("Volume must be positive", nameof(volume));

            var marketOrder = new MarketOrder(action, volume);

            _marketOrders.Add(marketOrder);

            AddHandlersAndExecute(marketOrder);

            return marketOrder;
        }

        private void AddHandlersAndExecute(MarketOrder marketOrder)
        {
            var tradeRequest = new TradeRequest
            {
                Volume = marketOrder.Volume
            };

            if (marketOrder.Action == Algo.OrderAction.Buy)
                AddHandler(marketOrder, tradeRequest, _tradingService.Buy);
            else
                AddHandler(marketOrder, tradeRequest, _tradingService.Sell);
        }

        private void AddHandler(
            MarketOrder marketOrder,
            ITradeRequest tradeRequest,
            Func<ITradeRequest, Task<ResponseModel<double>>> executor)
        {
            var task = executor(tradeRequest).WithTimeout();

            task.ContinueWith(previous =>
            {
                if (previous.IsCompletedSuccessfully)
                    HandleResponse(previous.Result, marketOrder, tradeRequest);
                else if (previous.IsFaulted)
                {
                    var ex = previous.Exception;
                    var error = previous.Exception.Message;

                    _userLogService.Enqueue(
                        _settingsService.GetInstanceId(),
                        $"There was a problem placing your order: {tradeRequest}. Error: {error}. ");

                    foreach (var inner in ex.Flatten().InnerExceptions)
                    {
                        if (inner is System.Net.Sockets.SocketException || inner is System.IO.IOException)
                            marketOrder.MarkErrored(TradeErrorCode.NetworkError, "");

                        if (inner is TaskCanceledException || inner is OperationCanceledException)
                            marketOrder.MarkErrored(TradeErrorCode.RequestTimeout, "");
                    }
                }
                else if (previous.IsCanceled)
                    marketOrder.MarkErrored(TradeErrorCode.RequestTimeout, "");
            });
        }

        private void HandleResponse(
            ResponseModel<double> result,
            MarketOrder marketOrder,
            ITradeRequest tradeRequest)
        {
            var isBuy = marketOrder.Action == Algo.OrderAction.Buy;
            var action = isBuy ? "buy" : "sell";

            if (result.Error != null)
            {
                _userLogService.Enqueue(_settingsService.GetInstanceId(),
                    $"There was a problem placing a {action} order. Error: {result.Error.Message} " +
                    $"is buying - {isBuy} ");

                marketOrder.MarkErrored(result.Error.ToTradeErrorCode(), result.Error.Message);
                return;
            }

            if (result.Result > 0)
            {
                _statisticsService.OnAction(isBuy, tradeRequest.Volume, result.Result);

                var dateTime = _settingsService.GetInstanceType() == Models.Enumerators.AlgoInstanceType.Test
                    ? _currentDataProvider.CurrentTimestamp
                    : DateTime.UtcNow;

                var tradedAssetId = _settingsService.GetTradedAssetId();
                var assetPairId = _settingsService.GetAlgoInstanceAssetPairId();

                _userLogService.Enqueue(
                    _settingsService.GetInstanceId(),
                    $"A {action} order successful: {tradeRequest.Volume} {tradedAssetId} - " +
                    $"price {result.Result} at {dateTime.ToDefaultDateTimeFormat()}");

                var tradeChartingUpdate = new TradeChartingUpdate
                {
                    Amount = tradeRequest.Volume,
                    AssetId = tradedAssetId,
                    AssetPairId = assetPairId,
                    DateOfTrade = dateTime,
                    InstanceId = _settingsService.GetInstanceId(),
                    IsBuy = isBuy,
                    Price = result.Result,
                    Id = Guid.NewGuid().ToString(),
                    OrderType = Models.Enumerators.OrderType.Market
                };

                _eventCollector.SubmitTradeEvent(tradeChartingUpdate).GetAwaiter().GetResult();

                marketOrder.MarkFulfilled();
                return;
            }

            marketOrder.MarkErrored(TradeErrorCode.Runtime, "Unexpected (empty) response.");
        }
    }
}
