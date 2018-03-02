using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels
{
    public class AlgoMetaDataFunction
    {
        public string Type { get; set; }
        public string Id { get; set; }

        public IEnumerable<AlgoMetaDataParameter> Parameters { get; set; }
    }
}
