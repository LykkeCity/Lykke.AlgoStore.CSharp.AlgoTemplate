using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Collections.Generic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Orders
{
    internal class LimitOrder : ILimitOrder
    {
        private readonly HashSet<Action<ILimitOrder, IContext>> _fulfilledCallbacks = new HashSet<Action<ILimitOrder, IContext>>();
        private readonly HashSet<Action<ILimitOrder, IContext>> _partiallyfulfilledCallbacks = new HashSet<Action<ILimitOrder, IContext>>();
        private readonly HashSet<Action<ILimitOrder, IContext>> _registeredCallbacks = new HashSet<Action<ILimitOrder, IContext>>();        
        private readonly HashSet<Action<ILimitOrder, IContext>> _cancelledCallbacks = new HashSet<Action<ILimitOrder, IContext>>();
        private readonly HashSet<Action<ILimitOrder, TradeErrorCode, string, IContext>> _erroredCallbacks
            = new HashSet<Action<ILimitOrder, TradeErrorCode, string, IContext>>();

        public OrderAction Action { get; }
        public Guid Id { get; set; }
        public double Volume { get; }
        public double VolumeFulfilled { get; set; }
        public double Price { get; }
        public OrderStatus Status { get; set; }

        public event Action<ILimitOrder, IContext> OnFulfilled
        {
            add => AddHandler(value, _fulfilledCallbacks);
            remove => RemoveHandler(value, _fulfilledCallbacks);
        }

        public event Action<ILimitOrder, IContext> OnPartiallyFulfilled
        {
            add => AddHandler(value, _partiallyfulfilledCallbacks);
            remove => RemoveHandler(value, _partiallyfulfilledCallbacks);
        }

        public event Action<ILimitOrder, IContext> OnRegistered
        {
            add => AddHandler(value, _registeredCallbacks);
            remove => RemoveHandler(value, _registeredCallbacks);
        }

        public event Action<ILimitOrder, IContext> OnCancelled
        {
            add => AddHandler(value, _cancelledCallbacks);
            remove => RemoveHandler(value, _cancelledCallbacks);
        }

        public event Action<ILimitOrder, TradeErrorCode, string, IContext> OnErrored
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

        public void MarkFulfilled(IContext context)
        {
            ValidateAndSetStatus(OrderStatus.Matched, "Order already fulfilled");

            this.VolumeFulfilled = Volume;
            foreach (var callback in _fulfilledCallbacks)
                callback(this, context);
        }

        public void MarkPartiallyFulfilled(double amountFullfilled, IContext context)
        {
            Status = OrderStatus.PartiallyMatched; // limit order can be partially fulfilled multiple times, no need to verify status
            
            this.VolumeFulfilled += amountFullfilled;
            foreach (var callback in _partiallyfulfilledCallbacks)
                callback(this, context);
        }

        public void MarkPlaced(IContext context)
        {
            ValidateAndSetStatus(OrderStatus.Placed, "Order already registered");

            foreach (var callback in _registeredCallbacks)
                callback(this, context);
        }

        public void MarkCancelled(IContext context)
        {
            ValidateAndSetStatus(OrderStatus.Cancelled, "Order already cancelled");

            foreach (var callback in _cancelledCallbacks)
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
