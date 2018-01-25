namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    public interface IAlgo
    {
        void OnQuoteReceived(IContext context);
    }
}
