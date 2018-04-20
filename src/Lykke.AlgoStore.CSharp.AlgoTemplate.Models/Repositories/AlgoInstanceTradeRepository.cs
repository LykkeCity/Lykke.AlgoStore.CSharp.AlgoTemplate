using AzureStorage;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Entities;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories
{
    public class AlgoInstanceTradeRepository : IAlgoInstanceTradeRepository
    {
        public static readonly string TableName = "AlgoInstanceTrades";

        private readonly INoSQLTableStorage<AlgoInstanceTradeEntity> _tableStorage;

        public AlgoInstanceTradeRepository(INoSQLTableStorage<AlgoInstanceTradeEntity> tableStorage)
        {
            _tableStorage = tableStorage;
        }

        private string GeneratePartitionKeyByOrderId(string orderId)
        {
            return orderId;
        }

        private string GenerateRowKeyByOrderId(string walletId)
        {
            return walletId;
        }

        public async Task<AlgoInstanceTrade> GetAlgoInstanceOrderAsync(string orderId, string walletId)
        {
            var partitionKey = GeneratePartitionKeyByOrderId(orderId);
            var rowKey = GenerateRowKeyByOrderId(walletId);

            var result = await _tableStorage.GetDataAsync(partitionKey, rowKey);

            return AutoMapper.Mapper.Map<AlgoInstanceTrade>(result);
        }

        public async Task CreateAlgoInstanceOrderAsync(AlgoInstanceTrade data)
        {
            var entity = AutoMapper.Mapper.Map<AlgoInstanceTradeEntity>(data);
            entity.PartitionKey = GeneratePartitionKeyByOrderId(data.OrderId);
            entity.RowKey = GenerateRowKeyByOrderId(data.WalletId);

            await _tableStorage.InsertAsync(entity);
        }
    }
}
