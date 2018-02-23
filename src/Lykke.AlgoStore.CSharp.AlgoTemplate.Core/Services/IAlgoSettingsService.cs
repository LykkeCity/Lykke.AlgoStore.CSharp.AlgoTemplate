namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IAlgoSettingsService
    {
        void Initialise();
        string GetMetadataSetting(string key);
        string GetAlgoInstanceClientId();
        string GetAlgoInstanceAssetPair();
        string GetAlgoInstanceTradedAsset();
    }
}
