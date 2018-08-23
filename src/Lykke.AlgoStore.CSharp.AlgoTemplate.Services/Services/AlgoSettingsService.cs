using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Newtonsoft.Json;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IAlgoSettingsService"/> implementation for the algo settings
    /// </summary>
    public class AlgoSettingsService : IAlgoSettingsService
    {
        private string _settingsJson;
        private IDictionary<string, object> _settings;

        private readonly IAlgoClientInstanceRepository _algoClientInstanceMetadataRepository;
        private string _authToken;
        private string _instanceId;
        private string _algoId;
        private string _tradedAssetId;
        private string _walletId;
        private AlgoInstanceType _instanceType;
        private AlgoClientInstanceData _clientInstanceData;

        public string GetAuthToken() => _authToken;
        public string GetAlgoId() => _algoId;
        public string GetInstanceId() => _instanceId;
        public string GetTradedAssetId() => _tradedAssetId;
        public string GetWalletId() => _walletId;
        public AlgoInstanceType GetInstanceType() => _instanceType;

        public AlgoSettingsService(IAlgoClientInstanceRepository algoClientInstanceMetadataRepository)
        {
            _algoClientInstanceMetadataRepository = algoClientInstanceMetadataRepository;

            _settingsJson = Environment.GetEnvironmentVariable("ALGO_INSTANCE_PARAMS");

            if (String.IsNullOrEmpty(_settingsJson))
                throw new ArgumentException("Environment variable 'ALGO_INSTANCE_PARAMS' does not contain settings");

            dynamic dynamicSettings = JsonConvert.DeserializeObject<ExpandoObject>(_settingsJson);
            _settings = (IDictionary<string, object>)dynamicSettings;

            _instanceId = GetSetting("InstanceId");
            _algoId = GetSetting("AlgoId");
            _authToken = GetSetting("AuthToken");
            _tradedAssetId = GetAlgoInstanceTradedAssetId();
            _walletId = GetAlgoInstanceWalletId();
            _instanceType = (AlgoInstanceType)Enum.Parse(typeof(AlgoInstanceType), GetSetting("InstanceType"));
            _clientInstanceData = _algoClientInstanceMetadataRepository.GetAlgoInstanceDataByAlgoIdAsync(_algoId, _instanceId).Result;
        }

        public void Initialize()
        {
            
        }

        public string GetSetting(string key)
        {
            if (!_settings.ContainsKey(key))
                return string.Empty;

            return _settings[key] as string;
        }

        public async Task UpdateAlgoInstance(AlgoClientInstanceData data)
        {
            await _algoClientInstanceMetadataRepository.SaveAlgoInstanceDataAsync(data);
            _clientInstanceData = data;
        }

        public AlgoClientInstanceData GetAlgoInstance() => _clientInstanceData;

        public string GetMetadataSetting(string key)
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceMetadataSetting(_algoId, _instanceId, key).Result;
        }

        private string GetAlgoInstanceWalletId() => _clientInstanceData.WalletId;
        private string GetAlgoInstanceTradedAssetId() => _clientInstanceData.TradedAssetId;

        public string GetAlgoInstanceAssetPairId() => _clientInstanceData.AssetPairId;
        public bool IsAlgoInstanceMarketOrderStraight() => _clientInstanceData.IsStraight;
        public string GetAlgoInstanceClientId() => _clientInstanceData.ClientId;
        public string GetAlgoInstanceOppositeAssetId() => _clientInstanceData.OppositeAssetId;
    }
}
