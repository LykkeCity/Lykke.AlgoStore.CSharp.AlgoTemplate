
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class AlgoInstanceStoppingData : BaseAlgoInstance
    {
        public AlgoInstanceStatus AlgoInstanceStatus { get; set; }

        public string EndOnDateTicks { get; set; }
    }
}
