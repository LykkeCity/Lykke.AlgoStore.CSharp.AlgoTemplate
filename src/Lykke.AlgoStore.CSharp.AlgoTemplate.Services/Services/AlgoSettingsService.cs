using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Newtonsoft.Json;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IAlgoSettingsService"/> implementation for the algo settings
    /// </summary>
    public class AlgoSettingsService : IAlgoSettingsService
    {
        private string _settingsJson;
        private IDictionary<string, object> _settings;
        private bool _isAlive;

        public bool IsAlive() => _isAlive; 
        private readonly IAlgoClientInstanceRepository _algoClientInstanceMetadataRepository;
        private string _instanceId;
        private string _algoId;

        public AlgoSettingsService(IAlgoClientInstanceRepository algoClientInstanceMetadataRepository)
        {
            _algoClientInstanceMetadataRepository = algoClientInstanceMetadataRepository;
        }

        //public AlgoSettingsService()
        //{

        //}

        public void Initialize()
        {
            _settingsJson = Environment.GetEnvironmentVariable("ALGO_INSTANCE_PARAMS");

            Console.WriteLine($"ALGO_INSTANCE_PARAMS: {_settingsJson}");

            if (String.IsNullOrEmpty(_settingsJson))
                throw new ArgumentException("Environment variable 'ALGO_INSTANCE_PARAMS' does not contain settings");

            dynamic dynamicSettings = JsonConvert.DeserializeObject<ExpandoObject>(_settingsJson);
            _settings = (IDictionary<string, object>)dynamicSettings;

            _instanceId = GetSetting("InstanceId");
            _algoId = GetSetting("AlgoId");

            _isAlive = true;

        }

        public string GetSetting(string key)
        {
            if (!_settings.ContainsKey(key))
                return string.Empty;

            return _settings[key] as string;
        }

        public string GetMetadataSetting(string key)
        {
           return  _algoClientInstanceMetadataRepository.GetAlgoInstanceMetadataSetting(_algoId,_instanceId, key).Result;
        }

        public string GetAlgoInstanceWalletId()
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceWalletId(_algoId, _instanceId).Result ?? string.Empty;
        }

        public string GetAlgoInstanceTradedAsset()
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceTradedAsset(_algoId, _instanceId).Result ?? string.Empty;
        }

        public string GetAlgoInstanceAssetPair()
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceAssetPair(_algoId, _instanceId).Result ?? string.Empty;
        }

        public string GetAlgoInstanceClientId()
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceClientId(_algoId, _instanceId).Result ?? string.Empty;
        }
    }
}
