using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper
{
    public static class KeyGenerator
    {
        private const string AlgoPartitionKeyStatic = "algo";
        private const string ClientPartitionKeyStatic = "client";
        private const string PartitionKeySeparator = "_";
        private const string PartitionKeyPattern = "{0}{1}{2}";       

        public static string GenerateClientIdPartitionKey(string clientId)
        {
            return string.Format(PartitionKeyPattern, ClientPartitionKeyStatic, PartitionKeySeparator, clientId);
        }

        public static string GenerateAlgoIdPartitionKey(string algoId)
        {
            return string.Format(PartitionKeyPattern, AlgoPartitionKeyStatic, PartitionKeySeparator, algoId);
        }

        public static string GenerateAlgoIdAndClientIdPartitionKey(string algoId, string clientId)
        {
            return string.Format(PartitionKeyPattern, algoId, PartitionKeySeparator, clientId);
        }

        public static BaseAlgoData ParseKey(string partitionKey)
        {
            var values = partitionKey.Split(PartitionKeySeparator);
            if (values == null || values.Length != 2)
                return null;

            if (values[0] == AlgoPartitionKeyStatic)
            {
                return new BaseAlgoData
                {
                    ClientId = null,
                    AlgoId = values[1]
                };
            }
            else if (values[0] == ClientPartitionKeyStatic)
            {
                return new BaseAlgoData
                {
                    ClientId = values[1],
                    AlgoId = null
                };
            }
            else return null;     
        }
    }
}
