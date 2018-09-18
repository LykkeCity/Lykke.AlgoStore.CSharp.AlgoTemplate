﻿using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Orders;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain.Contracts;
using Lykke.AlgoStore.MatchingEngineAdapter.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderAction = Lykke.AlgoStore.Algo.OrderAction;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="ITradingService"/> implementation
    /// </summary>
    public class TradingService : ITradingService
    {
        /// <summary>
        /// This class is placeholder for sample exception and 
        /// should be renamed/removed
        /// </summary>
        public class TradingServiceException : Exception { }

        private readonly IMatchingEngineAdapterClient _matchingEngineAdapterClient;
        private readonly IAlgoSettingsService _algoSettingsService;
        private readonly IFakeTradingService _fakeTradingService;

        private string _assetPairId;
        private string _walletId;
        private string _instanceId;
        private bool _straight;

        public TradingService(IMatchingEngineAdapterClient matchingEngineAdapterClient,
            IAlgoSettingsService algoSettingsService, IFakeTradingService fakeTradingService)
        {
            _matchingEngineAdapterClient = matchingEngineAdapterClient;
            _algoSettingsService = algoSettingsService;
            _fakeTradingService = fakeTradingService;
        }

        public async Task Initialize()
        {
            _instanceId = _algoSettingsService.GetInstanceId();
            _assetPairId = _algoSettingsService.GetAlgoInstanceAssetPairId();
            _walletId = _algoSettingsService.GetWalletId();
            _straight = _algoSettingsService.IsAlgoInstanceMarketOrderStraight();

            if (_algoSettingsService.GetInstanceType() == AlgoInstanceType.Live)
            {
                _matchingEngineAdapterClient.SetAuthToken(_algoSettingsService.GetAuthToken());
            }
            else
            {
                await _fakeTradingService.Initialize(_instanceId, _assetPairId, _straight);
            }
        }

        public async Task<ResponseModel<double>> Sell(ITradeRequest tradeRequest)
        {
            if (_algoSettingsService.GetInstanceType() == AlgoInstanceType.Live)
            {
                var meaResponse = await _matchingEngineAdapterClient.PlaceMarketOrderAsync(_walletId, _assetPairId,
                    MatchingEngineAdapter.Abstractions.Domain.OrderAction.Sell, tradeRequest.Volume, _straight, _instanceId, null);

                return meaResponse;
            }
            else
            {
                return await _fakeTradingService.Sell(tradeRequest);
            }
        }

        public async Task<ResponseModel<double>> Buy(ITradeRequest tradeRequest)
        {
            if (_algoSettingsService.GetInstanceType() == AlgoInstanceType.Live)
            {
                var meaResponse = await _matchingEngineAdapterClient.PlaceMarketOrderAsync(_walletId, _assetPairId,
                    MatchingEngineAdapter.Abstractions.Domain.OrderAction.Buy, tradeRequest.Volume, _straight, _instanceId, null);

                return meaResponse;
            }
            else
            {
                return await _fakeTradingService.Buy(tradeRequest);
            }
        }

        public async Task<ResponseModel<LimitOrderResponseModel>> PlaceLimitOrderAsync(ITradeRequest tradeRequest, OrderAction orderAction)
        {
            if (_algoSettingsService.GetInstanceType() == AlgoInstanceType.Live)
            {
                var meaResponse = await _matchingEngineAdapterClient.PlaceLimitOrderAsync(_walletId, _assetPairId, orderAction.ToMeaOrderAction(),
                    tradeRequest.Volume, tradeRequest.Price, _instanceId);

                return meaResponse;
            }           
            else
            {
                return await _fakeTradingService.PlaceLimitOrderAsync(tradeRequest, orderAction == OrderAction.Buy);
            }
        }

        public async Task<ResponseModel> CancelLimiOrderAsync(Guid limitOrderId)
        {
            var response = await _matchingEngineAdapterClient.CancelLimitOrderAsync(limitOrderId, _instanceId);

            return response;
        }
    }
}
