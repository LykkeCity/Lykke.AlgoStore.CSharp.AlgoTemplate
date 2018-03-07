using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels
{
    public class AlgoMetaDataParameter
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        public IEnumerable<EnumValue> PredefinedValues { get; set; }
    }
}
