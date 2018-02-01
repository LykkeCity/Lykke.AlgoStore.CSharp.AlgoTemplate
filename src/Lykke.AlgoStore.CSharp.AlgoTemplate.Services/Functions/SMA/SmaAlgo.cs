using System;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.SMA
{
    /// <summary>
    /// Simple Moving Average (SMA) function
    /// </summary>
    public class SmaAlgo
    {
        /// <summary>
        /// Parameters <see cref="SmaParameters"/>
        /// </summary>
        public SmaParameters Parameters { get; set; }

        /// <summary>
        /// Long term SMA function
        /// </summary>
        public SmaFunction LongTermSma { get; set; }

        /// <summary>
        /// Short term SMA function
        /// </summary>
        public SmaFunction ShortTermSma { get; set; }

        public override string ToString()
        {
            return $"Value={ShortTermSma.GetValue()}::ShortTermSma={GetShortTermSmaValue()}::LongTermSma={GetLongTermSmaValue()}";
        }

        /// <summary>
        /// Warm up short and long term SMA functions with data
        /// REMARK: We should use List&lt;Candle&gt; instead of double[]
        /// </summary>
        /// <param name="values">Data that will be used to warm up SMA functions</param>
        public void WarmUp(double[] values)
        {
            if(values.Length < Parameters.LongTermPeriod || values.Length < Parameters.ShortTermPeriod)
                throw new ArgumentOutOfRangeException();

            LongTermSma = new SmaFunction();
            LongTermSma.WarmUp(values.Skip(values.Length - Parameters.LongTermPeriod).Take(Parameters.LongTermPeriod).ToArray());

            ShortTermSma = new SmaFunction();
            ShortTermSma.WarmUp(values.Skip(values.Length - Parameters.ShortTermPeriod).Take(Parameters.ShortTermPeriod).ToArray());
        }

        /// <summary>
        /// Update short and long term SMA functions with latest quote data 
        /// REMARK: We should use IAlgoQuote instead of double
        /// </summary>
        /// <param name="value"></param>
        public void OnQuote(double value)
        {
            LongTermSma.AddNewValue(value);
            ShortTermSma.AddNewValue(value);
        }

        /// <summary>
        /// Get calculated SMA value based on long period (window)
        /// </summary>
        /// <returns></returns>
        public double GetLongTermSmaValue()
        {
            return LongTermSma.GetSmaValue();
        }

        /// <summary>
        /// Get calculated SMA value based on short period (window)
        /// </summary>
        /// <returns></returns>
        public double GetShortTermSmaValue()
        {
            return ShortTermSma.GetSmaValue();
        }
    }
}
