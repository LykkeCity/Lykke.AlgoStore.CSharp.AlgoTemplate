using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Extensions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderAction = Lykke.AlgoStore.Algo.OrderAction;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Orders
{
    public class LimitOrderManager : ILimitOrderManager
    {
        private readonly ITradingService _tradingService;
        private readonly IAlgoSettingsService _settingsService;
        private readonly IStatisticsService _statisticsService;
        private readonly IUserLogService _userLogService;
        private readonly IEventCollector _eventCollector;
        private readonly ICurrentDataProvider _currentDataProvider;

        private readonly List<LimitOrder> _limitOrders = new List<LimitOrder>();

        #region IReadOnlyList implementation

        public ILimitOrder this[int index] => _limitOrders[index];
        public int Count => _limitOrders.Count;

        public IEnumerator<ILimitOrder> GetEnumerator() => _limitOrders.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _limitOrders.GetEnumerator();

        #endregion // IReadOnlyList implementation

        public LimitOrderManager(
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

        public ILimitOrder Create(OrderAction action, double volume, double price)
        {
            if (volume <= 0)
                throw new ArgumentException("Volume must be positive", nameof(volume));

            var limitOrder = new LimitOrder(action, volume, price);
       
            AddHandlersAndExecute(limitOrder);

            _limitOrders.Add(limitOrder);

            return limitOrder;
        }

        public ILimitOrder Cancel(Guid limitOrderId)
        {
            var limitOrder = _limitOrders.First(lo => lo.Id == limitOrderId);

            AddCancelHandler(limitOrder, _tradingService.CancelLimiOrderAsync);

            return limitOrder;
        }

        private void AddHandlersAndExecute(LimitOrder limitOrder)
        {
            var tradeRequest = new TradeRequest
            {
                Volume = limitOrder.Volume,
                Price = limitOrder.Price
            };

            AddHandler(limitOrder, tradeRequest, _tradingService.PlaceLimitOrderAsync);
        }

        private void AddHandler(
            LimitOrder limitOrder,
            ITradeRequest tradeRequest,
            Func<ITradeRequest, OrderAction, Task<ResponseModel<LimitOrderResponseModel>>> executor)
        {
            var task = executor(tradeRequest, limitOrder.Action).WithTimeout();

            task.ContinueWith(previous =>
            {
                if (previous.IsCompletedSuccessfully)
                    HandleResponse(previous.Result, limitOrder, tradeRequest);
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
                            limitOrder.MarkErrored(TradeErrorCode.NetworkError, "");

                        if (inner is TaskCanceledException || inner is OperationCanceledException)
                            limitOrder.MarkErrored(TradeErrorCode.RequestTimeout, "");
                    }
                }
                else if (previous.IsCanceled)
                    limitOrder.MarkErrored(TradeErrorCode.RequestTimeout, "");
            });
        }

        private void HandleResponse(
            ResponseModel<LimitOrderResponseModel> result,
            LimitOrder limitOrder,
            ITradeRequest tradeRequest)
        {
            var isBuy = limitOrder.Action == OrderAction.Buy;
            var action = isBuy ? "buy" : "sell";

            if (result.Error != null)
            {
                _userLogService.Enqueue(_settingsService.GetInstanceId(),
                    $"There was a problem placing a {action} order. Error: {result.Error.Message} " +
                    $"is buying - {isBuy} ");

                limitOrder.MarkErrored(result.Error.ToTradeErrorCode(), result.Error.Message);
                return;
            }
            else if (result.Error == null)
            {
                var dateTime = _settingsService.GetInstanceType() == Models.Enumerators.AlgoInstanceType.Test
                    ? _currentDataProvider.CurrentTimestamp
                    : DateTime.UtcNow;
   
                _userLogService.Enqueue(
                    _settingsService.GetInstanceId(),
                    $"A {action} limit order successful: {tradeRequest.Volume} - " + 
                    $"price {tradeRequest.Price} at {dateTime.ToDefaultDateTimeFormat()}");

                limitOrder.Id = result.Result.Id;
                limitOrder.MarkPlaced();
                return;
            }

            limitOrder.MarkErrored(TradeErrorCode.Runtime, "Unexpected (empty) response.");
        }

        private void AddCancelHandler(LimitOrder limitOrder, Func<Guid, Task<ResponseModel>> executor)
        {
            var task = executor(limitOrder.Id).WithTimeout();

            task.ContinueWith(previous =>
            {
                if (previous.IsCompletedSuccessfully)
                    HandleCancelResponse(previous.Result, limitOrder);
                else if (previous.IsFaulted)
                {
                    var ex = previous.Exception;
                    var error = previous.Exception.Message;

                    _userLogService.Enqueue(
                        _settingsService.GetInstanceId(),
                        $"There was a problem cancelling your limit order: Id: {limitOrder.Id}. Error: {error}. ");

                    foreach (var inner in ex.Flatten().InnerExceptions)
                    {
                        if (inner is System.Net.Sockets.SocketException || inner is System.IO.IOException)
                            limitOrder.MarkErrored(TradeErrorCode.NetworkError, "");

                        if (inner is TaskCanceledException || inner is OperationCanceledException)
                            limitOrder.MarkErrored(TradeErrorCode.RequestTimeout, "");
                    }
                }
                else if (previous.IsCanceled)
                    limitOrder.MarkErrored(TradeErrorCode.RequestTimeout, "");
            });
        }

        private void HandleCancelResponse(
            ResponseModel result,
            LimitOrder limitOrder)
        {
            if (result.Error != null)
            {
                _userLogService.Enqueue(_settingsService.GetInstanceId(),
                    $"There was a problem canceling a limit order with Id - {limitOrder.Id}. Error: {result.Error.Message} ");

                limitOrder.MarkErrored(result.Error.ToTradeErrorCode(), result.Error.Message);
                return;
            }
            else if (result.Error == null)
            {
                var dateTime = _settingsService.GetInstanceType() == Models.Enumerators.AlgoInstanceType.Test
                    ? _currentDataProvider.CurrentTimestamp
                    : DateTime.UtcNow;

                _userLogService.Enqueue(
                    _settingsService.GetInstanceId(),
                    $"A limit order cancellation successful: limit order Id - {limitOrder.Id}" +
                    $" at {dateTime.ToDefaultDateTimeFormat()}");

                limitOrder.MarkCancelled();
                return;
            }

            limitOrder.MarkErrored(TradeErrorCode.Runtime, "Unexpected (empty) response.");
        }
    }
}
