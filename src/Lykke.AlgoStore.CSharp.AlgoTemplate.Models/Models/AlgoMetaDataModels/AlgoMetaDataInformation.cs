using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels
{
    public class AlgoMetaDataInformation
    {
        public IList<AlgoMetaDataParameter> Parameters { get; set; }
        public IList<AlgoMetaDataFunction> Functions { get; set; }
    }
}
