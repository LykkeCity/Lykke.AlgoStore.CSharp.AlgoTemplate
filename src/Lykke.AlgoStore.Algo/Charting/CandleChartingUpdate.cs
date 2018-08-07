using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.Algo.Charting
{
    public class CandleChartingUpdate : Candle, IChartingUpdate
    {
        public string InstanceId { get; set; }

        public string AssetPair { get; set; }

        public CandleTimeInterval CandleTimeInterval { get; set; }
    }
}
