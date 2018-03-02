using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lykke.Service.Assets.Client.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IAssetServiceDecorator
    {
        Task<AssetPair> GetEnabledAssetPairAsync(string assetPairId);
        Task<IEnumerable<AssetPair>> GetAllEnabledAssetPairsAsync();
        Task<Asset> GetEnabledAssetAsync(string assetPairId);
        Task<IEnumerable<Asset>> GetAllEnabledAssetsAsync();
    }
}
