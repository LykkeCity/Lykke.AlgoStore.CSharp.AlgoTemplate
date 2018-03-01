namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    public class BaseAlgo : IAlgo
    {
        public virtual void OnCandleReceived(ICandleContext context)
        {
        }

        public virtual void OnQuoteReceived(IQuoteContext context)
        {
        }

        public virtual void OnStartUp(IFunctionProvider functions)
        {
        }
    }
}
