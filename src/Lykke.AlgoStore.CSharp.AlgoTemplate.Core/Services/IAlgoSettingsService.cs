namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IAlgoSettingsService
    {
        void Initialize();
        string GetSetting(string key);
        bool IsAlive();
    }
}
