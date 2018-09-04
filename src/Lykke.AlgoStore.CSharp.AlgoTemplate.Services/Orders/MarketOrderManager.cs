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
    public sealed class MarketOrderManager : IMarketOrderManager
    {
        private readonly ITradingService _tradingService;
        private readonly IAlgoSettingsService _settingsService;
        private readonly IStatisticsService _statisticsService;
        private readonly IUserLogService _userLogService;
        private readonly IEventCollector _eventCollector;
        private readonly ICurrentDataProvider _currentDataProvider;

        private readonly List<MarketOrder> _marketOrders = new List<MarketOrder>();
        private readonly HashSet<Task> _pendingTasks = new HashSet<Task>();
        private readonly object _sync = new object();

        private bool _isDisposing;

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
            // This check will handle the case where algo trade callbacks might create more trades
            if (_isDisposing)
                throw new ObjectDisposedException(nameof(MarketOrderManager));

            if (volume <= 0)
                throw new ArgumentException("Volume must be positive", nameof(volume));

            var marketOrder = new MarketOrder(action, volume);

            _marketOrders.Add(marketOrder);

            AddHandlersAndExecute(marketOrder);

            return marketOrder;
        }

        public void Dispose()
        {
            if(_isDisposing) return;

            _isDisposing = true;

            try
            {
                Task.WhenAll(_pendingTasks).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception) { }
            // We're disposing, ignore all uncaught exceptions to prevent interrupting the shutdown process
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
            var tradeTask = executor(tradeRequest).WithTimeout();

            var tradeHandleTask = tradeTask.ContinueWith(async previous =>
            {
                if (previous.IsCompletedSuccessfully)
                    await HandleResponse(previous.Result, marketOrder, tradeRequest);
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

            // Ensure that all market orders and their response handling will complete before the
            // instance shutdown by keeping track of all non-completed tasks and awaiting them when disposing

            lock(_sync)
            {
                _pendingTasks.Add(tradeHandleTask);
            }

            tradeHandleTask.ContinueWith((t) =>
            {
                if (_isDisposing) return;

                lock(_sync)
                {
                    _pendingTasks.Remove(t);
                }
            });
        }

        private async Task HandleResponse(
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

                marketOrder.MarkErrored(result.Error.Code.ToTradeErrorCode(), result.Error.Message);
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
                    Id = Guid.NewGuid().ToString()
                };

                await _eventCollector.SubmitTradeEvent(tradeChartingUpdate);

                marketOrder.MarkFulfilled();
                return;
            }

            marketOrder.MarkErrored(TradeErrorCode.Runtime, "Unexpected (empty) response.");
        }
    }
}
