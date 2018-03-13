using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;

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

        AlgoClientInstanceData GetAlgoInstance();

        string GetAlgoId();
        string GetInstanceId();
        string GetTradedAsset();
    }
}
