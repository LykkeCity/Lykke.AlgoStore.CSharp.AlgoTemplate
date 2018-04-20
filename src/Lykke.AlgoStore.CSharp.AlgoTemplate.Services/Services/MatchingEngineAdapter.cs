﻿using Common.Log;
using JetBrains.Annotations;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings;
using Lykke.MatchingEngine.Connector.Abstractions.Models;
using Lykke.MatchingEngine.Connector.Abstractions.Services;
using Lykke.Service.Assets.Client;
using Lykke.Service.FeeCalculator.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FeeType = Lykke.Service.FeeCalculator.AutorestClient.Models.FeeType;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class MatchingEngineAdapter : IMatchingEngineAdapter
    {
        private readonly ILog _log;
        private readonly IMatchingEngineClient _matchingEngineClient;
        private readonly IFeeCalculatorClient _feeCalculatorClient;
        private readonly IAssetsService _assetsService;
        private readonly FeeSettings _feeSettings;

        private static readonly Dictionary<MeStatusCodes, ResponseModel.ErrorCodeType> StatusCodesMap = new Dictionary<MeStatusCodes, ResponseModel.ErrorCodeType>
        {
            {MeStatusCodes.LowBalance, ResponseModel.ErrorCodeType.LowBalance},
            {MeStatusCodes.AlreadyProcessed, ResponseModel.ErrorCodeType.AlreadyProcessed},
            {MeStatusCodes.UnknownAsset, ResponseModel.ErrorCodeType.UnknownAsset},
            {MeStatusCodes.NoLiquidity, ResponseModel.ErrorCodeType.NoLiquidity},
            {MeStatusCodes.NotEnoughFunds, ResponseModel.ErrorCodeType.NotEnoughFunds},
            {MeStatusCodes.Dust, ResponseModel.ErrorCodeType.Dust},
            {MeStatusCodes.ReservedVolumeHigherThanBalance, ResponseModel.ErrorCodeType.ReservedVolumeHigherThanBalance},
            {MeStatusCodes.NotFound, ResponseModel.ErrorCodeType.NotFound},
            {MeStatusCodes.BalanceLowerThanReserved, ResponseModel.ErrorCodeType.BalanceLowerThanReserved},
            {MeStatusCodes.LeadToNegativeSpread, ResponseModel.ErrorCodeType.LeadToNegativeSpread},
            {MeStatusCodes.TooSmallVolume, ResponseModel.ErrorCodeType.Dust},
            {MeStatusCodes.InvalidFee, ResponseModel.ErrorCodeType.Runtime},
            {MeStatusCodes.Duplicate, ResponseModel.ErrorCodeType.Runtime},
            {MeStatusCodes.Runtime, ResponseModel.ErrorCodeType.Runtime}
        };

        public MatchingEngineAdapter(IMatchingEngineClient matchingEngineClient,
            IFeeCalculatorClient feeCalculatorClient,
            IAssetsService assetsService,
            FeeSettings feeSettings,
            [NotNull] ILog log)
        {
            _matchingEngineClient =
                matchingEngineClient ?? throw new ArgumentNullException(nameof(matchingEngineClient));
            _feeCalculatorClient = feeCalculatorClient ?? throw new ArgumentNullException(nameof(feeCalculatorClient));
            _assetsService = assetsService ?? throw new ArgumentNullException(nameof(assetsService));
            _feeSettings = feeSettings ?? throw new ArgumentNullException(nameof(feeSettings));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<ResponseModel> CancelLimitOrderAsync(Guid limitOrderId)
        {
            var response = await _matchingEngineClient.CancelLimitOrderAsync(limitOrderId.ToString());
            await CheckResponseAndThrowIfNull(response);

            return ConvertToApiModel(response.Status);
        }

        public async Task<ResponseModel<double>> HandleMarketOrderAsync(string clientId, string assetPairId, OrderAction orderAction, double volume,
            bool straight, double? reservedLimitVolume = null)
        {
            var order = new MarketOrderModel
            {
                Id = GetNextRequestId().ToString(),
                AssetPairId = assetPairId,
                ClientId = clientId,
                ReservedLimitVolume = reservedLimitVolume,
                Straight = straight,
                Volume = Math.Abs(volume),
                OrderAction = orderAction.ToMeOrderAction(),
                Fees = new[] { await GetMarketOrderFee(clientId, assetPairId, orderAction) }
            };

            var response = await _matchingEngineClient.HandleMarketOrderAsync(order);
            await CheckResponseAndThrowIfNull(response);
            if (response.Status == MeStatusCodes.Ok)
            {
                return ResponseModel<double>.CreateOk(response.Price);
            }
            return ConvertToApiModel<double>(response.Status);
        }


        //This method is to be used in the future when implementing Limit orders.
        //public async Task<ResponseModel<Guid>> PlaceLimitOrderAsync(string clientId, string assetPairId, Core.Domain.OrderAction orderAction, double volume,
        //    double price, bool cancelPreviousOrders = false)
        //{
        //    var requestId = GetNextRequestId();

        //    await _orderStateRepository.Add(new LimitOrderState { Id = requestId, ClientId = clientId, AssetPairId = assetPairId, Volume = volume, Price = price });

        //    var order = new LimitOrderModel
        //    {
        //        Id = requestId.ToString(),
        //        AssetPairId = assetPairId,
        //        ClientId = clientId,
        //        Price = price,
        //        CancelPreviousOrders = cancelPreviousOrders,
        //        Volume = Math.Abs(volume),
        //        OrderAction = orderAction.ToMeOrderAction(),
        //        Fee = await GetLimitOrderFee(clientId, assetPairId, orderAction)
        //    };

        //    var response = await _matchingEngineClient.PlaceLimitOrderAsync(order);
        //    await CheckResponseAndThrowIfNull(response);
        //    if (response.Status == MeStatusCodes.Ok)
        //    {
        //        return ResponseModel<Guid>.CreateOk(requestId);
        //    }

        //    var responseModel = ConvertToApiModel<Guid>(response.Status);
        //    responseModel.Result = requestId;
        //    return responseModel;
        //}

        private Guid GetNextRequestId()
        {
            return Guid.NewGuid();
        }

        private async Task CheckResponseAndThrowIfNull(object response)
        {
            if (response == null)
            {
                var exception = new InvalidOperationException("ME not available");
                await _log.WriteErrorAsync(nameof(MatchingEngineAdapter), nameof(CancelLimitOrderAsync), exception);
                throw exception;
            }
        }

        private ResponseModel ConvertToApiModel(MeStatusCodes status)
        {
            if (status == MeStatusCodes.Ok)
                return ResponseModel.CreateOk();

            return ResponseModel.CreateFail(StatusCodesMap[status]);
        }

        private ResponseModel<T> ConvertToApiModel<T>(MeStatusCodes status)
        {
            return ResponseModel<T>.CreateFail(StatusCodesMap[status]);
        }

        private async Task<MarketOrderFeeModel> GetMarketOrderFee(string clientId, string assetPairId, OrderAction orderAction)
        {
            var assetPair = await _assetsService.AssetPairGetAsync(assetPairId);
            var fee = await _feeCalculatorClient.GetMarketOrderAssetFee(clientId, assetPairId, assetPair?.BaseAssetId, orderAction.ToFeeOrderAction());

            return new MarketOrderFeeModel
            {
                Size = (double)fee.Amount,
                SizeType = fee.Type == FeeType.Absolute
                    ? (int)FeeSizeType.ABSOLUTE
                    : (int)FeeSizeType.PERCENTAGE,
                SourceClientId = clientId,
                TargetClientId = fee.TargetWalletId ?? _feeSettings.TargetClientId.Hft,
                Type = fee.Amount == 0m
                    ? (int)MarketOrderFeeType.NO_FEE
                    : (int)MarketOrderFeeType.CLIENT_FEE,
                AssetId = string.IsNullOrEmpty(fee.TargetAssetId)
                    ? Array.Empty<string>()
                    : new[] { fee.TargetAssetId }
            };
        }

        //To be used in future when implementing Limit orders
        //private async Task<LimitOrderFeeModel> GetLimitOrderFee(string clientId, string assetPairId, Core.Domain.OrderAction orderAction)
        //{
        //    var assetPair = await _assetsService.AssetPairGetAsync(assetPairId);
        //    var fee = await _feeCalculatorClient.GetLimitOrderFees(clientId, assetPairId, assetPair?.BaseAssetId, orderAction.ToFeeOrderAction());

        //    return new LimitOrderFeeModel
        //    {
        //        MakerSize = (double)fee.MakerFeeSize,
        //        TakerSize = (double)fee.TakerFeeSize,
        //        SourceClientId = clientId,
        //        TargetClientId = _feeSettings.TargetClientId.Hft,
        //        Type = (int)LimitOrderFeeType.CLIENT_FEE
        //    };
        //}
    }
}
