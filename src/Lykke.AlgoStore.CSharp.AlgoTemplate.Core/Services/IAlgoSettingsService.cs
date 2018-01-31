namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IAlgoSettingsService
    {
        void Initialise();
        string GetSetting(string key);
    }
}
