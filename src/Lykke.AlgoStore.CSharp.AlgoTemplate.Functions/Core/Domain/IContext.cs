namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Core.Domain
{
    /// <summary>
    /// An algo context. Represents a user consumable resources from an algo.
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Access point to functions and function results for an
        /// algo <see cref="IFunctionProvider"/>
        /// </summary>
        IFunctionProvider Functions { get; }

        /// <summary>
        /// Access to statistics information and service available to
        /// an algo. <see cref="IStatistics"/>
        /// </summary>
        IStatistics Statistics { get; }
    }
}
