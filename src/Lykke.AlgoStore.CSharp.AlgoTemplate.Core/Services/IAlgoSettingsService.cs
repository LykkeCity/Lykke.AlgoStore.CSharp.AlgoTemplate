using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IAlgoSettingsService
    {
        void Initialize();
        string GetSetting(string key);
        bool IsAlive();

        string GetMetadataSetting(string key);
        string GetAlgoInstanceWalletId();
        string GetAlgoInstanceAssetPair();
        string GetAlgoInstanceClientId();
        bool IsAlgoInstanceMarketOrderStraight();

        AlgoClientInstanceData GetAlgoInstance();

        Task UpdateAlgoInstance(AlgoClientInstanceData data);

        string GetAlgoId();
        string GetInstanceId();
        string GetTradedAsset();
    }
}
