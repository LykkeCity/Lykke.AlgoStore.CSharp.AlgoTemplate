using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.Algo.Indicators
{
    /// <summary>
    /// Abstract implementation of <see cref="IIndicator"/> which provides
    /// ability to the function to be evaluated with MIN/MAX/OPEN/CLOSE
    /// values of a <see cref="Candle"/>
    /// </summary>
    public abstract class AbstractIndicator : BaseIndicator
    {
        /// <summary>
        /// The candle value on which the function is operating. The 
        /// same function can be operating on Min/Max or Open/Close
        /// of a Candle
        /// </summary>
        public CandleOperationMode CandleOperationMode { get; }

        /// <summary>
        /// Base exception for function invocation exceptions
        /// </summary>
        public class FunctionInvocationException : Exception
        {
            public FunctionInvocationException(string message, Exception innerException)
                : base(message, innerException) { }
        }

        /// <summary>
        /// <see cref="FunctionInvocationException"/> with a value
        /// </summary>
        public class AddNewValueException : FunctionInvocationException
        {
            public Candle ValueInvokedWith { get; set; }

            public AddNewValueException(string message, Exception innerException)
                : base(message, innerException) { }
        }

        /// <summary>
        /// <see cref="FunctionInvocationException"/> with a warm-up value
        /// </summary>
        public class WarmUpException : FunctionInvocationException
        {
            public IEnumerable<Candle> ValueInvokedWith { get; set; }

            public WarmUpException(string message, Exception innerException)
                : base(message, innerException) { }
        }

        private Dictionary<CandleOperationMode, Func<Candle, double>> _candleProjections
            = new Dictionary<CandleOperationMode, Func<Candle, double>>()
            {
                { CandleOperationMode.OPEN, (Candle c) => c.Open },
                { CandleOperationMode.CLOSE, (Candle c) => c.Close },
                { CandleOperationMode.LOW, (Candle c) => c.Low },
                { CandleOperationMode.HIGH, (Candle c) => c.High }
            };

        public AbstractIndicator(
            DateTime startingDate,
            DateTime endingDate,
            CandleTimeInterval candleTimeInterval,
            string assetPair,
            CandleOperationMode candleOperationMode)
            : base(startingDate, endingDate, candleTimeInterval, assetPair)
        {
            CandleOperationMode = candleOperationMode;
        }

        /// <summary>
        /// Initialize the function with the initial values.
        /// </summary>
        /// <param name="values">The initial values to be 
        /// computed by the function</param>
        public abstract double? WarmUp(IEnumerable<double> values);

        public override double? WarmUp(IEnumerable<Candle> values)
        {
            try
            {
                var candleProjection = _candleProjections[CandleOperationMode];
                return WarmUp((values ?? new List<Candle>()).Select(candleProjection));
            }
            catch (Exception e)
            {
                throw new WarmUpException($"Exception thrown while warming up function '{GetType().Name}'", e)
                {
                    ValueInvokedWith = values
                };
            }
        }

        /// <summary>
        /// Re-calculate the function with the new value
        /// </summary>
        /// <param name="value"></param>
        public abstract double? AddNewValue(double value);

        public override double? AddNewValue(Candle value)
        {
            if (value == null)
            {
                throw new AddNewValueException("Invalid value of null provided for add new value", null)
                {
                    ValueInvokedWith = value
                };
            }

            try
            {
                // Extract the corresponding double value out of a Candle
                var candleProjection = _candleProjections[CandleOperationMode];
                return AddNewValue(candleProjection(value));
            }
            catch (Exception e)
            {
                throw new AddNewValueException($"Exception thrown while adding new value for a function '{GetType().Name}'", e)
                {
                    ValueInvokedWith = value
                };
            }
        }
    }
}
