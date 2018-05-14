using Lykke.AlgoStore.CSharp.Algo.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    /// <summary>
    /// The <see cref="IContext"/> implementation provided to an algo when running.
    /// </summary>
    public class Context : IContext
    {
        /// <summary>
        /// Access point to functions and function results for an
        /// algo <see cref="IFunctionProvider"/>
        /// </summary>
        public IFunctionProvider Functions { get; set; }

        ///// <summary>
        ///// Access point for the user action, which can be performed 
        ///// by an algo <see cref="IActions"/>
        ///// </summary>
        //public IActions Actions { get; set; }

        /// <summary>
        /// Access to statistics information and service available to
        /// an algo. <see cref="IStatistics"/>
        /// </summary>
        public IStatistics Statistics { get; set; }
    }
}
