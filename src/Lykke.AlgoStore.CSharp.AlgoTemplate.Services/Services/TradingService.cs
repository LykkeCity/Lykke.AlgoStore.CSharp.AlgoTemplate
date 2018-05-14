using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;
using Lykke.AlgoStore.MatchingEngineAdapter.Client;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="ITradingService"/> implementation
    /// </summary>
    public class TradingService : ITradingService
    {
        /// <summary>
        /// This class is placeholder for sample exception and 
        /// should be renamed/removed
        /// </summary>
        public class TradingServiceException : Exception { }

        private readonly IMatchingEngineAdapterClient _matchingEngineAdapterClient;
        private readonly IAlgoSettingsService _algoSettingsService;
        private readonly IFakeTradingService _fakeTradingService;

        private string _assetPairId;
        private string _walletId;
        private string _instanceId;
        private bool _straight;

        public TradingService(IMatchingEngineAdapterClient matchingEngineAdapterClient,
            IAlgoSettingsService algoSettingsService, IFakeTradingService fakeTradingService)
        {
            _matchingEngineAdapterClient = matchingEngineAdapterClient;
            _algoSettingsService = algoSettingsService;
            _fakeTradingService = fakeTradingService;
        }

        public void Initialize()
        {
            _instanceId = _algoSettingsService.GetInstanceId();
            _assetPairId = _algoSettingsService.GetAlgoInstanceAssetPair();
            _walletId = _algoSettingsService.GetAlgoInstanceWalletId();
            _straight = _algoSettingsService.IsAlgoInstanceMarketOrderStraight();

            if (_algoSettingsService.GetAlgoInstance().AlgoInstanceType == AlgoInstanceType.Live)
            {
                //_matchingEngineAdapterClient.SetClientAndInstanceId(_algoSettingsService.GetAlgoInstanceClientId(), _instanceId);
            }
            else
            {
                _fakeTradingService.Initialize(_instanceId,_assetPairId, _straight);
            }
        }

        public async Task<ResponseModel<double>> Sell(double volume, IAlgoCandle candleData)
        {

            if (_algoSettingsService.GetAlgoInstance().AlgoInstanceType == AlgoInstanceType.Live)
            {

                //var meaResponse = await _matchingEngineAdapterClient.PlaceMarketOrder(_walletId, _assetPairId,
                //    OrderAction.Sell, volume, _straight, _instanceId, null);

                //return meaResponse;
                return null;
            }
            else
            {

                return null;
            }
        }

        public async Task<ResponseModel<double>> Buy(double volume, IAlgoCandle candleData)
        {
            if (_algoSettingsService.GetAlgoInstance().AlgoInstanceType == AlgoInstanceType.Live)
            {
                //var meaResponse = await _matchingEngineAdapterClient.PlaceMarketOrder(_walletId, _assetPairId,
                //    OrderAction.Buy, volume, _straight, _instanceId, null);

                //return meaResponse;
                return null;
            }
            else
            {

                return null;
            }
        }
    }
}
