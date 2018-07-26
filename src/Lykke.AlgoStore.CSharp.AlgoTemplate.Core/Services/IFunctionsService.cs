using System.Collections.Generic;
using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    /// <summary>
    /// Interface for a service handling the functions <see cref="IFunction"/>
    /// </summary>
    public interface IFunctionsService
    {
        /// <summary>
        /// Initializes the service.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Generate <see cref="CandleServiceRequest"/> request needed for the function
        /// executions
        /// </summary>
        /// <returns></returns>
        IEnumerable<CandleServiceRequest> GetCandleRequests(string authToken);

        /// <summary>
        /// Perform a warm-up of all function. This will feed the function with historical
        /// data. <see cref="IFunction.WarmUp(Candle)"/>
        /// </summary>
        /// <param name="candles"></param>
        void WarmUp(IList<MultipleCandlesResponse> candles);

        /// <summary>
        /// Recalculates the function with a new candle data provided. 
        /// <see cref="IFunction.AddNewValue(Candle)"/>
        /// </summary>
        /// <param name="candles"></param>
        void Recalculate(IList<SingleCandleResponse> candles);

        /// <summary>
        /// Gets the current function results
        /// </summary>
        /// <returns></returns>
        IFunctionProvider GetFunctionResults();
    }
}
