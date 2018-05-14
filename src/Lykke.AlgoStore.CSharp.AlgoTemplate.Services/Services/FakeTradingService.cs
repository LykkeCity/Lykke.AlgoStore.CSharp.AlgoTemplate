using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class FakeTradingService : IFakeTradingService
    {
        private string _assetPairId;
        private string _tradedAssetId;
        private string _instanceId;
        private bool _straight;

        private readonly IAlgoSettingsService _algoSettingsService;
        private readonly IAlgoInstanceTradeRepository _algoInstanceTradeRepository;
        private readonly IStatisticsRepository _statisticsRepository;

        public FakeTradingService(IAlgoSettingsService algoSettingsService,
            IAlgoInstanceTradeRepository algoInstanceTradeRepository,
            IStatisticsRepository statisticsRepository)
        {
            _algoSettingsService = algoSettingsService;
            _algoInstanceTradeRepository = algoInstanceTradeRepository;
            _statisticsRepository = statisticsRepository;
        }

        public void Initialize(string instanceId, string assetPairId, bool straight)
        {
            _instanceId = instanceId;
            _assetPairId = assetPairId;
            _straight = straight;
            _tradedAssetId = _algoSettingsService.GetTradedAsset();
        }

        public async Task<ResponseModel<double>> Sell(double volume, IAlgoCandle candleData)
        {
            //save traded asset trade
            await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(new AlgoInstanceTrade()
            {
                AssetPairId = _assetPairId,
                Amount = -volume,
                Price = candleData.Close,
                DateOfTrade = candleData.DateTime,
                IsBuy = false,
                AssetId = _tradedAssetId
            });

            //save oposite asset trade of traded asset
            double tradedOpositeVolume = CalculateOpositeOfTradedAssetTradeValue(volume, candleData.Close);

            await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(new AlgoInstanceTrade()
            {
                AssetPairId = _assetPairId,
                Amount = tradedOpositeVolume,
                Price = candleData.Close,
                IsBuy = false,
            });

            var summary = await _statisticsRepository.GetSummaryAsync(_instanceId);
            summary.LastTradedAssetBalance -= volume;
            summary.LastAssetTwoBalance += tradedOpositeVolume;

            return null;
        }

        public async Task<ResponseModel<double>> Buy(double volume, IAlgoCandle candleData)
        {
            double tradedOpositeVolume = CalculateOpositeOfTradedAssetTradeValue(volume, candleData.Close);

            //save traded asset trade
            await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(new AlgoInstanceTrade()
            {
                AssetPairId = _assetPairId,
                AssetId = _tradedAssetId,
                Amount = volume,
                Price = candleData.Close,
                DateOfTrade = candleData.DateTime,
                IsBuy = true
            });

            //save oposite asset trade of traded asset
            await _algoInstanceTradeRepository.SaveAlgoInstanceTradeAsync(new AlgoInstanceTrade()
            {
                AssetPairId = _assetPairId,
                AssetId = "oposite Asset Id",//can we keep it in the database 
                Amount = -tradedOpositeVolume,
                Price = candleData.Close,
                DateOfTrade = candleData.DateTime,
                IsBuy = true
            });

            var summary = await _statisticsRepository.GetSummaryAsync(_instanceId);
            summary.LastTradedAssetBalance += volume;
            summary.LastAssetTwoBalance -= tradedOpositeVolume;

            return null;
        }

        private double CalculateOpositeOfTradedAssetTradeValue(double volume, double closePrice)
        {
            double tradedOpositeVolume = 0;
            if (_straight)
                tradedOpositeVolume = volume * closePrice;
            else
                tradedOpositeVolume = volume / closePrice;

            return tradedOpositeVolume;
        }
    }
}
