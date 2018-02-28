using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lykke.AlgoStore.CSharp.Algo.Core.Functions
{
    /// <summary>
    /// Abstract implementation of <see cref="IFunction"/> which provides
    /// ability to the function to be evaluated with MIN/MAX/OPEN/CLOSE
    /// values of a <see cref="Candle"/>
    /// </summary>
    public abstract class AbstractFunction : IFunction
    {
        /// <summary>
        /// Base exception for function invocation exceptions
        /// </summary>
        public class FunctionInvocationException : Exception
        {
            public FunctionParamsBase FunctionParameters { get; set; }

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
            public IList<Candle> ValueInvokedWith { get; set; }

            public WarmUpException(string message, Exception innerException)
                : base(message, innerException) { }
        }

        /// <summary>
        /// The inputs of a function. <see cref="FunctionParamsBase"/>
        /// </summary>
        public abstract FunctionParamsBase FunctionParameters { get; }

        /// <summary>
        /// The latest value of a function.
        /// </summary>
        public abstract double? Value { get; }

        private Dictionary<FunctionParamsBase.CandleValue, Func<Candle, double>> _candleProjections
            = new Dictionary<FunctionParamsBase.CandleValue, Func<Candle, double>>()
            {
                { FunctionParamsBase.CandleValue.OPEN, (Candle c) => c.Open },
                { FunctionParamsBase.CandleValue.CLOSE, (Candle c) => c.Close },
                { FunctionParamsBase.CandleValue.LOW, (Candle c) => c.Low },
                { FunctionParamsBase.CandleValue.HIGH, (Candle c) => c.High }
            };

        /// <summary>
        /// Initialize the function with the initial values.
        /// </summary>
        /// <param name="values">The initial values to be 
        /// computed by the function</param>
        abstract public double? WarmUp(double[] values);

        public double? WarmUp(IList<Candle> values)
        {
            try
            {
                var candleProjection = _candleProjections[FunctionParameters.CandleOperationMode];
                return WarmUp((values ?? new List<Candle>()).Select(candleProjection).ToArray());
            }
            catch (Exception e)
            {
                throw new WarmUpException($"Exception thrown while warming up function '{GetType().Name}'", e)
                {
                    FunctionParameters = FunctionParameters,
                    ValueInvokedWith = values
                };
            }
        }

        /// <summary>
        /// Re-calculate the function with the new value
        /// </summary>
        /// <param name="value"></param>
        abstract public double? AddNewValue(double value);

        public double? AddNewValue(Candle value)
        {
            if (value == null)
            {
                throw new AddNewValueException("Invalid value of null provided for add new value", null)
                {
                    FunctionParameters = FunctionParameters,
                    ValueInvokedWith = value
                };
            }

            try
            {
                // Extract the corresponding double value out of a Candle
                var candleProjection = _candleProjections[FunctionParameters.CandleOperationMode];
                return AddNewValue(candleProjection(value));
            }
            catch (Exception e)
            {
                throw new AddNewValueException($"Exception thrown while adding new value for a function '{GetType().Name}'", e)
                {
                    FunctionParameters = FunctionParameters,
                    ValueInvokedWith = value
                };
            }
        }
    }
}
