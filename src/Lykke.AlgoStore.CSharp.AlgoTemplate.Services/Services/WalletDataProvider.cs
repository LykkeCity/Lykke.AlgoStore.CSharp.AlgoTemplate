using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.Service.InstanceBalance.Client;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class WalletDataProvider : IWalletDataProvider
    {
        private IInstanceBalanceClient _balanceClient;
        private IAlgoSettingsService _algoSettingsService;
        private IFakeTradingService _fakeTradingService;

        public WalletDataProvider(
            IInstanceBalanceClient balanceClient,
            IAlgoSettingsService algoSettingsService,
            IFakeTradingService fakeTradingService)
        {
            _balanceClient = balanceClient;
            _algoSettingsService = algoSettingsService;
            _fakeTradingService = fakeTradingService;
        }

        public Dictionary<string, WalletBalance> GetBalances()
        {
            if(_algoSettingsService.GetInstanceType() == Models.Enumerators.AlgoInstanceType.Live)
            {
                var assets = _balanceClient.Api
                    .GetBalancesAsync("Bearer " + _algoSettingsService.GetAuthToken())
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                return assets.ToDictionary(a => a.AssetId, a => new WalletBalance
                {
                    Balance = a.Balance,
                    Reserved = a.Reserved
                });
            }

            return new Dictionary<string, WalletBalance>
            {
                [_algoSettingsService.GetTradedAssetId()] = new WalletBalance
                {
                    Balance = _fakeTradingService.TradedAssetBalance,
                    Reserved = 0
                },
                [_algoSettingsService.GetAlgoInstanceOppositeAssetId()] = new WalletBalance
                {
                    Balance = _fakeTradingService.OppositeAssetBalance,
                    Reserved = 0
                }
            };
        }
    }
}
