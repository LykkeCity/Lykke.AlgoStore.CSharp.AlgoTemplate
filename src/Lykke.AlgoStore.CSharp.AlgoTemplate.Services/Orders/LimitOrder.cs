using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Orders
{
    internal class LimitOrder : ILimitOrder
    {
        private readonly HashSet<Action<ILimitOrder>> _fulfilledCallbacks = new HashSet<Action<ILimitOrder>>();
        private readonly HashSet<Action<ILimitOrder>> _registeredCallbacks = new HashSet<Action<ILimitOrder>>();        
        private readonly HashSet<Action<ILimitOrder>> _cancelledCallbacks = new HashSet<Action<ILimitOrder>>();
        private readonly HashSet<Action<ILimitOrder, TradeErrorCode, string>> _erroredCallbacks
            = new HashSet<Action<ILimitOrder, TradeErrorCode, string>>();

        public OrderAction Action { get; }
        public Guid Id { get; set; }
        public double Volume { get; }
        public double Price { get; }
        public OrderStatus Status { get; set; }

        public event Action<ILimitOrder> OnFulfilled
        {
            add => AddHandler(value, _fulfilledCallbacks);
            remove => RemoveHandler(value, _fulfilledCallbacks);
        }

        public event Action<ILimitOrder> OnRegistered
        {
            add => AddHandler(value, _registeredCallbacks);
            remove => RemoveHandler(value, _registeredCallbacks);
        }

        public event Action<ILimitOrder> OnCancelled
        {
            add => AddHandler(value, _cancelledCallbacks);
            remove => RemoveHandler(value, _cancelledCallbacks);
        }

        public event Action<ILimitOrder, TradeErrorCode, string> OnErrored
        {
            add => AddHandler(value, _erroredCallbacks);
            remove => RemoveHandler(value, _erroredCallbacks);
        }

        public LimitOrder(OrderAction action, double volume, double price)
        {
            Action = action;
            Volume = volume;
            Price = price;
            Status = OrderStatus.Pending;
        }

        public void MarkMatched()
        {
            ValidateAndSetStatus(OrderStatus.Matched, "Order already fulfilled");

            foreach (var callback in _fulfilledCallbacks)
                callback(this);
        }

        public void MarkPlaced()
        {
            ValidateAndSetStatus(OrderStatus.Placed, "Order already registered");

            foreach (var callback in _registeredCallbacks)
                callback(this);
        }

        public void MarkCancelled()
        {
            ValidateAndSetStatus(OrderStatus.Cancelled, "Order already cancelled");

            foreach (var callback in _cancelledCallbacks)
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
