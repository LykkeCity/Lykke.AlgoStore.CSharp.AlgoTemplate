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

            result.ClientId = entitiy.ClientId;
            result.AlgoId = entitiy.AlgoId;
            result.InstanceId = entitiy.RowKey;
            result.AssetPair = entitiy.AssetPair;
            result.HftApiKey = entitiy.HftApiKey;
            result.WalletId = entitiy.WalletId;
            result.Margin = entitiy.Margin;
            result.TradedAsset = entitiy.TradedAsset;
            result.Volume = entitiy.Volume;
            result.AlgoMetaDataInformation =
                JsonConvert.DeserializeObject<AlgoMetaDataInformation>(entitiy.AlgoMetaDataInformation);

            return result;
        }
        public static AlgoClientInstanceEntity ToEntityWithAlgoIdPartitionKey(this AlgoClientInstanceData data)
        {
            var result = new AlgoClientInstanceEntity();

            if (data == null)
                return result;

            result.PartitionKey = KeyGenerator.GenerateAlgoIdPartitionKey(data.AlgoId);
            result.RowKey = data.InstanceId;
            result.AssetPair = data.AssetPair;
            result.HftApiKey = data.HftApiKey;
            result.WalletId = data.WalletId;
            result.Margin = data.Margin;
            result.TradedAsset = data.TradedAsset;
            result.Volume = data.Volume;
            result.ETag = "*";
            result.ClientId = data.ClientId;
            result.AlgoId = data.AlgoId;
            result.AlgoMetaDataInformation = JsonConvert.SerializeObject(data.AlgoMetaDataInformation);
            return result;
        }

        public static AlgoClientInstanceEntity ToEntityWithClientIdPartitionKey(this AlgoClientInstanceData data)
        {
            var result = new AlgoClientInstanceEntity();

            if (data == null)
                return result;

            result.PartitionKey = KeyGenerator.GenerateClientIdPartitionKey(data.ClientId);
            result.RowKey = data.InstanceId;
            result.AssetPair = data.AssetPair;
            result.HftApiKey = data.HftApiKey;
            result.WalletId = data.WalletId;
            result.Margin = data.Margin;
            result.TradedAsset = data.TradedAsset;
            result.Volume = data.Volume;
            result.ETag = "*";
            result.ClientId = data.ClientId;
            result.AlgoId = data.AlgoId;
            result.AlgoMetaDataInformation = JsonConvert.SerializeObject(data.AlgoMetaDataInformation);
            return result;
        }
    }
}
