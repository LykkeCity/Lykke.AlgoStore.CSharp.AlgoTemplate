namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain
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
    }
}
