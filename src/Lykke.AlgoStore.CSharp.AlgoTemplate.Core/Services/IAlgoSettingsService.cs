using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IAlgoSettingsService
    {
        void Initialize();
        string GetSetting(string key);
        bool IsAlive();

        string GetMetadataSetting(string key);
        string GetAlgoInstanceWalletId();
        string GetAlgoInstanceAssetPairId();
        string GetAlgoInstanceClientId();
        string GetAlgoInstanceOppositeAssetId();
        bool IsAlgoInstanceMarketOrderStraight();

        AlgoClientInstanceData GetAlgoInstance();

        Task UpdateAlgoInstance(AlgoClientInstanceData data);

        string GetAuthToken();
        string GetAlgoId();
        string GetInstanceId();
        string GetTradedAsset();
        AlgoInstanceType GetInstanceType();
    }
}
