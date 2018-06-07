using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels
{
    public class AlgoMetaDataFunction
    {
        /// <summary>
        /// Namespace of the function, which will be used to load the function
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///Unique name of the function. We define it's id - name - by which we will call the function 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Type of parameters class. Namespace of the parameter class, which will be used to load the class
        /// </summary>
        public string FunctionParameterType { get; set; }

        /// <summary>
        /// Function Parameters
        /// </summary>
        public IList<AlgoMetaDataParameter> Parameters { get; set; }
    }
}
