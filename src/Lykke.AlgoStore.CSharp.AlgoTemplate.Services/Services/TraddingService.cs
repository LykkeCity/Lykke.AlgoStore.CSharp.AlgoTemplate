using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="ITradingService"/> implementation
    /// </summary>
    public class TraddingService : ITradingService
    {
        /// <summary>
        /// This class is placeholder for sample exception and 
        /// should be renamed/removed
        /// </summary>
        public class TradingServiceException : Exception { }

        public virtual double BuyReverse(double volume)
        {
            throw new NotImplementedException();
        }

        public virtual double BuyStraight(double volume)
        {
            throw new NotImplementedException();
        }

        public void Initialise()
        {
            throw new NotImplementedException();
        }

        public virtual double SellReverse(double volume)
        {
            throw new NotImplementedException();
        }

        public virtual double SellStraight(double volume)
        {
            throw new NotImplementedException();
        }
    }
}
