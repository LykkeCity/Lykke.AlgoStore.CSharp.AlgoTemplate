using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.AlgoMetaDataModels
{
    public class AlgoMetaDataInformation
    {
        public IEnumerable<AlgoMetaDataParameter> Parameters { get; set; }
        public IEnumerable<AlgoMetaDataFunction> Functions { get; set; }
    }
}
