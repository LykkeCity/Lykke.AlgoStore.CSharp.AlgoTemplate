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
        private bool _isAlive;

        public bool IsAlive() => _isAlive;
        private readonly IAlgoClientInstanceRepository _algoClientInstanceMetadataRepository;
        private string _instanceId;
        private string _algoId;
        private string _tradedAsset;
        private AlgoInstanceType _instanceType;

        public string GetAlgoId() => _algoId;
        public string GetInstanceId() => _instanceId;
        public string GetTradedAsset() => _tradedAsset;
        public AlgoInstanceType GetInstanceType() => _instanceType;

        //public AlgoInstanceType GetInstanceType()
        //{
        //    return _algoClientInstanceMetadataRepository.GetAlgoInstanceDataByAlgoIdAsync(_algoId, _instanceId).Result.AlgoInstanceType;
        //}

        public AlgoSettingsService(IAlgoClientInstanceRepository algoClientInstanceMetadataRepository)
        {
            _algoClientInstanceMetadataRepository = algoClientInstanceMetadataRepository;
        }

        public void Initialize()
        {
            _settingsJson = Environment.GetEnvironmentVariable("ALGO_INSTANCE_PARAMS");

            if (String.IsNullOrEmpty(_settingsJson))
                throw new ArgumentException("Environment variable 'ALGO_INSTANCE_PARAMS' does not contain settings");

            dynamic dynamicSettings = JsonConvert.DeserializeObject<ExpandoObject>(_settingsJson);
            _settings = (IDictionary<string, object>)dynamicSettings;

            _instanceId = GetSetting("InstanceId");
            _algoId = GetSetting("AlgoId");
            _tradedAsset = GetAlgoInstanceTradedAsset();
            _instanceType = (AlgoInstanceType)Enum.Parse(typeof(AlgoInstanceType), GetSetting("InstanceType"));
            _isAlive = true;
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
        }

        public AlgoClientInstanceData GetAlgoInstance()
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceDataByAlgoIdAsync(_algoId, _instanceId).Result;
        }

        public string GetMetadataSetting(string key)
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceMetadataSetting(_algoId, _instanceId, key).Result;
        }

        public string GetAlgoInstanceWalletId()
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceDataByAlgoIdAsync(_algoId, _instanceId).Result.WalletId;
        }

        private string GetAlgoInstanceTradedAsset()
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceDataByAlgoIdAsync(_algoId, _instanceId).Result.TradedAsset;
        }

        public string GetAlgoInstanceAssetPair()
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceDataByAlgoIdAsync(_algoId, _instanceId).Result.AssetPair;
        }

        public bool IsAlgoInstanceMarketOrderStraight()
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceDataByAlgoIdAsync(_algoId, _instanceId).Result.IsStraight;
        }

        public string GetAlgoInstanceClientId()
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceDataByAlgoIdAsync(_algoId, _instanceId).Result.ClientId;
        }

        public string GetAlgoInstanceOppositeAssetId()
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceDataByAlgoIdAsync(_algoId, _instanceId).Result.OppositeAssetId;
        }
    }
}
