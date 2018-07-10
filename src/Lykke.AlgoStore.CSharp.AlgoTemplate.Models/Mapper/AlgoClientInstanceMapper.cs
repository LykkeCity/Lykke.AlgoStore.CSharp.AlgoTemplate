using System;
using System.Globalization;
using System.Linq;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper
{
    public static class AlgoClientInstanceMapper
    {
        public static AlgoClientInstanceData ToModel(this AlgoClientInstanceEntity entitiy)
        {
            var result = new AlgoClientInstanceData();

            if (entitiy == null)
                return result;

            result = AutoMapper.Mapper.Map<AlgoClientInstanceData>(entitiy);

            result.AlgoMetaDataInformation = entitiy.AlgoMetaDataInformation != null ?
                                                JsonConvert.DeserializeObject<AlgoMetaDataInformation>(entitiy.AlgoMetaDataInformation)
                                                : new AlgoMetaDataInformation();
            return result;
        }

        public static AlgoClientInstanceEntity ToEntityWithAlgoIdPartitionKey(this AlgoClientInstanceData data)
        {
            var result = new AlgoClientInstanceEntity();

            if (data == null)
                return result;

            result = GetEntityResult(data, result);
            result.PartitionKey = KeyGenerator.GenerateAlgoIdPartitionKey(data.AlgoId);
            return result;
        }

        public static AlgoClientInstanceEntity ToEntityWithAlgoIdAndClientIdPartitionKey(this AlgoClientInstanceData data)
        {
            var result = new AlgoClientInstanceEntity();

            if (data == null)
                return result;

            result = GetEntityResult(data, result);
            result.PartitionKey = KeyGenerator.GenerateAlgoIdAndClientIdPartitionKey(data.AlgoId, data.ClientId);
            return result;
        }

        public static AlgoClientInstanceEntity ToEntityWithAlgoIdAndInstanceTypePartitionKey(this AlgoClientInstanceData data)
        {
            var result = new AlgoClientInstanceEntity();

            if (data == null)
                return result;

            result = GetEntityResult(data, result);
            result.PartitionKey = KeyGenerator.GenerateAlgoIdAndInstanceTypePartitionKey(data.AlgoId, data.AlgoInstanceType);
            return result;
        }

        public static AlgoClientInstanceEntity ToEntityWithWalletIdPartitionKey(this AlgoClientInstanceData data)
        {
            var result = new AlgoClientInstanceEntity();

            if (data == null)
                return result;

            result = GetEntityResult(data, result);
            result.PartitionKey = KeyGenerator.GenerateWalletIdPartitionKey(data.WalletId);
            return result;
        }

        public static AlgoClientInstanceEntity ToEntityWithClientIdPartitionKey(this AlgoClientInstanceData data)
        {
            var result = new AlgoClientInstanceEntity();

            if (data == null)
                return result;

            result = GetEntityResult(data, result);
            result.PartitionKey = KeyGenerator.GenerateClientIdPartitionKey(data.ClientId);
            return result;
        }

        public static AlgoClientInstanceEntity ToEntityWithAuthTokenPartitionKey(this AlgoClientInstanceData data)
        {
            var result = new AlgoClientInstanceEntity();

            if (data == null)
                return result;

            result = GetEntityResult(data, result);
            result.PartitionKey = KeyGenerator.GenerateAuthTokenPartitionKey(data.AuthToken);
            return result;
        }

        public static AlgoInstanceStoppingEntity ToStoppingEntityWithEndDatePartitionKey(this AlgoClientInstanceData data)
        {
            var result = new AlgoInstanceStoppingEntity();

            if (data == null)
                return result;

            var instanceEndDateString = data.AlgoMetaDataInformation.Parameters.Single(p => p.Key == "EndOn").Value;
            DateTime.TryParseExact(instanceEndDateString, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal, out var instanceEndDate);

            result = AutoMapper.Mapper.Map<AlgoInstanceStoppingEntity>(data);
            result.PartitionKey = KeyGenerator.GenerateStoppingEntityPartitionKey();
            result.RowKey = KeyGenerator.GenerateStoppingEntityRowKey(instanceEndDate);
            result.ETag = "*";    
            return result;
        }

        private static AlgoClientInstanceEntity GetEntityResult(AlgoClientInstanceData data, AlgoClientInstanceEntity result)
        {
            result = AutoMapper.Mapper.Map<AlgoClientInstanceEntity>(data);
            result.ETag = "*";
            result.AlgoMetaDataInformation = JsonConvert.SerializeObject(data.AlgoMetaDataInformation);
            return result;
        }
    }
}
