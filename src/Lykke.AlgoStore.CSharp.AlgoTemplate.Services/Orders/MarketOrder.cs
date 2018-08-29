using Lykke.AlgoStore.Algo;
using System;
using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Orders
{
    internal class MarketOrder : IMarketOrder
    {
        private readonly HashSet<Action<IMarketOrder>> _fulfilledCallbacks = new HashSet<Action<IMarketOrder>>();
        private readonly HashSet<Action<IMarketOrder, TradeErrorCode, string>> _erroredCallbacks
            = new HashSet<Action<IMarketOrder, TradeErrorCode, string>>();

        public OrderAction Action { get; }
        public double Volume { get; }
        public OrderStatus Status { get; set; }

        public event Action<IMarketOrder> OnFulfilled
        {
            add => AddHandler(value, _fulfilledCallbacks);
            remove => RemoveHandler(value, _fulfilledCallbacks);
        }

        public event Action<IMarketOrder, TradeErrorCode, string> OnErrored
        {
            add => AddHandler(value, _erroredCallbacks);
            remove => RemoveHandler(value, _erroredCallbacks);
        }

        public MarketOrder(OrderAction action, double volume)
        {
            Action = action;
            Volume = volume;
            Status = OrderStatus.Pending;
        }
        
        public void MarkFulfilled()
        {
            ValidateAndSetStatus(OrderStatus.Errored, "Order already fulfilled");

            foreach (var callback in _fulfilledCallbacks)
                callback(this);
        }

        public void MarkErrored(TradeErrorCode error, string message)
        {
            ValidateAndSetStatus(OrderStatus.Errored, "Order already errored");

            foreach (var callback in _erroredCallbacks)
                callback(this, error, message);
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        private void AddHandler<T>(T handler, HashSet<T> handlerSet)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            handlerSet.Add(handler);
        }

        private void RemoveHandler<T>(T handler, HashSet<T> handlerSet)
        {
            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            handlerSet.Remove(handler);
        }

        private void ValidateAndSetStatus(OrderStatus status, string errorMessage)
        {
            if (Status == status)
                throw new InvalidOperationException(errorMessage);

            Status = status;
        }
    }
}
