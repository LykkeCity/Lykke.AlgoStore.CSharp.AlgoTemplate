using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IAlgoSettingsService"/> implementation for the algo settings
    /// </summary>
    public class AlgoSettingsService : IAlgoSettingsService
    {
        private readonly IAlgoClientInstanceRepository _algoClientInstanceMetadataRepository;
        private readonly string _instanceId;
        private readonly string _algoId;


        public AlgoSettingsService(IAlgoClientInstanceRepository algoClientInstanceMetadataRepository,
            string instanceId, string algoId)
        {
            _algoClientInstanceMetadataRepository = algoClientInstanceMetadataRepository;
            _instanceId = instanceId;
            _algoId = algoId;
        }

        public void Initialise()
        {

        }

        public string GetMetadataSetting(string key)
        {
           return  _algoClientInstanceMetadataRepository.GetAlgoInstanceMetadataSetting(_algoId,_instanceId, key).Result;
        }

        public string GetAlgoInstanceClientId()
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceClientId(_algoId, _instanceId).Result;
        }

        public string GetAlgoInstanceAssetPair()
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceAssetPair(_algoId, _instanceId).Result;
        }

        public string GetAlgoInstanceTradedAsset()
        {
            return _algoClientInstanceMetadataRepository.GetAlgoInstanceTradedAsset(_algoId, _instanceId).Result;
        }

    }
}
