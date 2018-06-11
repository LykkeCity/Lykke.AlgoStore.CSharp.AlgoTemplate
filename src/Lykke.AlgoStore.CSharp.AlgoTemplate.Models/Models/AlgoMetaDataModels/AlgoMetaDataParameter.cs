using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels
{
    public class AlgoMetaDataParameter
    {
        /// <summary>
        /// Unique name of the parameter. This value should be similar to variable defined in code
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Parameter value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Optional parameter description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Type of the parameter - DateTime, String, int, double, etc.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// If parameter is enum, it has predefined values which will be used from front-end guys for visualization in dropdowns
        /// </summary>
        public IList<EnumValue> PredefinedValues { get; set; }
    }
}
