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
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
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
        private readonly ILimitOrderUpdateSubscriber _limitOrderUpdateSubscriber;

        private readonly List<(LimitOrder order, IContext context)> _limitOrders = new List<(LimitOrder order, IContext context)>(); //context is only used to provide access to logging and ordersProvider to the event handlers

        private bool _isDisposing;

        #region IReadOnlyList implementation

        public ILimitOrder this[int index] => _limitOrders[index].order;
        public int Count => _limitOrders.Count;

        public IEnumerator<ILimitOrder> GetEnumerator() => _limitOrders.Select(t=>t.order).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _limitOrders.Select(t => t.order).GetEnumerator();

        #endregion // IReadOnlyList implementation

        public LimitOrderManager(
            ITradingService tradingService,
            IAlgoSettingsService settingsService,
            IStatisticsService statisticsService,
            IUserLogService userLogService,
            IEventCollector eventCollector,
            ICurrentDataProvider currentDataProvider,
            ILimitOrderUpdateSubscriber limitOrderUpdateSubscriber)
        {
            _tradingService = tradingService;
            _settingsService = settingsService;
            _statisticsService = statisticsService;
            _userLogService = userLogService;
            _eventCollector = eventCollector;
            _currentDataProvider = currentDataProvider;
            _limitOrderUpdateSubscriber = limitOrderUpdateSubscriber;

            _limitOrderUpdateSubscriber.Subscribe(_settingsService.GetInstanceId(), ProcessLimitOrderUpdate);
        }

        private void ProcessLimitOrderUpdate(AlgoInstanceTrade tradeUpdate)
        {
            var orders = _limitOrders.Where(o => o.order.Id.ToString() == tradeUpdate.OrderId && tradeUpdate.InstanceId == _settingsService.GetInstanceId()).ToList();
            if (orders.Any())
            {
                var orderFound = orders.First();

                switch (tradeUpdate.OrderStatus)
                {
                    case OrderStatus.Matched:
                        orderFound.order.MarkFulfilled(orderFound.context);
                        break;
                    case OrderStatus.Cancelled:
                        orderFound.order.MarkCancelled(orderFound.context);
                        break;
                    case OrderStatus.PartiallyMatched:
                        orderFound.order.MarkPartiallyFulfilled(tradeUpdate.Amount ?? 0, orderFound.context);
                        break;
                    case OrderStatus.Placed:
                        orderFound.order.MarkPlaced(orderFound.context);
                        break;
                }
            }
        }

        public void Dispose()
        {
            if (_isDisposing) return;

            _isDisposing = true;

            try
            {
                _limitOrderUpdateSubscriber.Dispose();
            }
            catch (Exception)
            {
                // We're disposing, ignore all uncaught exceptions to prevent interrupting the shutdown process
            }
        }

        public ILimitOrder Create(OrderAction action, double volume, double price, IContext context)
        {
            if (volume <= 0)
                throw new ArgumentException("Volume must be positive", nameof(volume));

            var limitOrder = new LimitOrder(action, volume, price);
            
            AddHandlersAndExecute(limitOrder, context);

            _limitOrders.Add((order: limitOrder, context: context));

            return limitOrder;
        }

        public ILimitOrder Cancel(Guid limitOrderId)
        {
            var limitOrder = _limitOrders.First(lo => lo.order.Id == limitOrderId);

            AddCancelHandler(limitOrder.order, _tradingService.CancelLimiOrderAsync, limitOrder.context);

            return limitOrder.order;
        }

        private void AddHandlersAndExecute(LimitOrder limitOrder, IContext context)
        {
            var tradeRequest = new TradeRequest
            {
                Volume = limitOrder.Volume,
                Price = limitOrder.Price
            };

            AddHandler(limitOrder, tradeRequest, _tradingService.PlaceLimitOrderAsync, context);
        }

        private void AddHandler(
            LimitOrder limitOrder,
            ITradeRequest tradeRequest,
            Func<ITradeRequest, OrderAction, Task<ResponseModel<LimitOrderResponseModel>>> executor,
            IContext context)
        {
            var task = executor(tradeRequest, limitOrder.Action).WithTimeout();

            task.ContinueWith(previous =>
            {
                if (previous.IsCompletedSuccessfully)
                    HandleResponse(previous.Result, limitOrder, tradeRequest, context);
                else if (previous.IsFaulted)
                {
                    var ex = previous.Exception;
                    var error = previous.Exception.Message;

                    _userLogService.Enqueue(
                        _settingsService.GetInstanceId(),
                        $"There was a problem placing your order: {tradeRequest}. Error: {error}. ");

                    foreach (var inner in ex.Flatten().InnerExceptions)
                    {
                        if (inner is System.Net.Sockets.SocketException || inner is System.IO.IOException || inner is ObjectDisposedException)
                            limitOrder.MarkErrored(TradeErrorCode.NetworkError, "", context);

                        if (inner is TaskCanceledException || inner is OperationCanceledException)
                            limitOrder.MarkErrored(TradeErrorCode.RequestTimeout, "", context);
                    }
                }
                else if (previous.IsCanceled)
                    limitOrder.MarkErrored(TradeErrorCode.RequestTimeout, "", context);
            });
        }

        private void HandleResponse(
            ResponseModel<LimitOrderResponseModel> result,
            LimitOrder limitOrder,
            ITradeRequest tradeRequest,
            IContext context)
        {
            var isBuy = limitOrder.Action == OrderAction.Buy;
            var action = isBuy ? "buy" : "sell";

            if (result.Error != null)
            {
                _userLogService.Enqueue(_settingsService.GetInstanceId(),
                    $"There was a problem placing a {action} order. Error: {result.Error.Message} " +
                    $"is buying - {isBuy} ");

                limitOrder.MarkErrored(result.Error.ToTradeErrorCode(), result.Error.Message, context);
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
                limitOrder.MarkPlaced(context);
                return;
            }

            limitOrder.MarkErrored(TradeErrorCode.Runtime, "Unexpected (empty) response.", context);
        }

        private void AddCancelHandler(LimitOrder limitOrder, Func<Guid, Task<ResponseModel>> executor, IContext context)
        {
            var task = executor(limitOrder.Id).WithTimeout();

            task.ContinueWith(previous =>
            {
                if (previous.IsCompletedSuccessfully)
                    HandleCancelResponse(previous.Result, limitOrder, context);
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
                            limitOrder.MarkErrored(TradeErrorCode.NetworkError, "", context);

                        if (inner is TaskCanceledException || inner is OperationCanceledException)
                            limitOrder.MarkErrored(TradeErrorCode.RequestTimeout, "", context);
                    }
                }
                else if (previous.IsCanceled)
                    limitOrder.MarkErrored(TradeErrorCode.RequestTimeout, "", context);
            });
        }

        private void HandleCancelResponse(
            ResponseModel result,
            LimitOrder limitOrder,
            IContext context)
        {
            if (result.Error != null)
            {
                _userLogService.Enqueue(_settingsService.GetInstanceId(),
                    $"There was a problem canceling a limit order with Id - {limitOrder.Id}. Error: {result.Error.Message} ");

                limitOrder.MarkErrored(result.Error.ToTradeErrorCode(), result.Error.Message, context);
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

                limitOrder.MarkCancelled(context);
                return;
            }

            limitOrder.MarkErrored(TradeErrorCode.Runtime, "Unexpected (empty) response.", context);
        }
    }
}
