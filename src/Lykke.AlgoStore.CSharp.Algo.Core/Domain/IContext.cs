namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    public interface IContext
    {
        IData Data { get; set; }
        ISettings Settings { get; set; }
        IFunctions Functions { get; set; }
        IActions Actions { get; set; }
        IStatistics Statistics { get; set; }
    }
}
