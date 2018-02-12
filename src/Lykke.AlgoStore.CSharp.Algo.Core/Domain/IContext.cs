namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    /// <summary>
    /// An algo context. Represents a user consumable resources from an algo.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Access to the data needed by an algo to run
        /// <see cref="IData"/>
        /// </summary>
        IData Data { get; }

        /// <summary>
        /// Access point to functions and function results for an
        /// algo <see cref="IFunctionsResultsProvider"/>
        /// </summary>
        IFunctionsResultsProvider Functions { get; }

        /// <summary>
        /// Access point for the user action, which can be performed 
        /// by an algo <see cref="IActions"/>
        /// </summary>
        IActions Actions { get; }

        /// <summary>
        /// Access to statistics information and service available to
        /// an algo. <see cref="IStatistics"/>
        /// </summary>
        IStatistics Statistics { get; }
    }
}
