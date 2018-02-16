﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class AssetServiceDecorator : IAssetServiceDecorator
    {
        private readonly IAssetsServiceWithCache _apiService;

        public AssetServiceDecorator(IAssetsServiceWithCache apiService)
        {
            _apiService = apiService;
        }

        public async Task<AssetPair> GetEnabledAssetPairAsync(string assetPairId)
        {
            var pair = await _apiService.TryGetAssetPairAsync(assetPairId);

            return pair == null || pair.IsDisabled ? null : pair;
        }

        public async Task<IEnumerable<AssetPair>> GetAllEnabledAssetPairsAsync()
        {
            return (await _apiService.GetAllAssetPairsAsync()).Where(a => !a.IsDisabled);
        }


        public async Task<Asset> GetEnabledAssetAsync(string assetId)
        {
            var asset = await _apiService.TryGetAssetAsync(assetId);

            return asset == null || asset.IsDisabled ? null : asset;
        }

        public async Task<IEnumerable<Asset>> GetAllEnabledAssetsAsync()
        {
            return (await _apiService.GetAllAssetsAsync()).Where(a => !a.IsDisabled);
        }
    }
}
