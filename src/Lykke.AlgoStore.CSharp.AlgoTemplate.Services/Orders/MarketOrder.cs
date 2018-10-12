using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Orders
{
    internal class MarketOrder : IMarketOrder
    {
        private readonly HashSet<Action<IMarketOrder, IContext>> _fulfilledCallbacks = new HashSet<Action<IMarketOrder, IContext>>();
        private readonly HashSet<Action<IMarketOrder, TradeErrorCode, string, IContext>> _erroredCallbacks
            = new HashSet<Action<IMarketOrder, TradeErrorCode, string, IContext>>();

        public OrderAction Action { get; }
        public double Volume { get; }
        public OrderStatus Status { get; set; }

        public event Action<IMarketOrder, IContext> OnFulfilled
        {
            add => AddHandler(value, _fulfilledCallbacks);
            remove => RemoveHandler(value, _fulfilledCallbacks);
        }

        public event Action<IMarketOrder, TradeErrorCode, string, IContext> OnErrored
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
        
        public void MarkFulfilled(IContext context)
        {
            ValidateAndSetStatus(OrderStatus.Matched, "Order already fulfilled");

            foreach (var callback in _fulfilledCallbacks)
                callback(this, context);
        }

        public void MarkErrored(TradeErrorCode error, string message, IContext context)
        {
            ValidateAndSetStatus(OrderStatus.Errored, "Order already errored");

            foreach (var callback in _erroredCallbacks)
                callback(this, error, message, context);
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
