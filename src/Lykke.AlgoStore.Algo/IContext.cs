namespace Lykke.AlgoStore.Algo
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

        IOrderProvider Orders { get; }

        /// <summary>
        /// Access point for the user action, which can be performed 
        /// by an algo <see cref="IActions"/>
        /// </summary>
        IActions Actions { get; }
    }
}
