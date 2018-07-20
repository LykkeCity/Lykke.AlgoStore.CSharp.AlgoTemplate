using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions
{
    public static class ErrorCodeTypeExtensions
    {
        public static TradeErrorCode ToTradeErrorCode(this ResponseModel.ErrorCodeType meaErrorCodeType)
        {
            switch (meaErrorCodeType)
            {
                case ResponseModel.ErrorCodeType.LowBalance:
                    return TradeErrorCode.LowBalance;
                case ResponseModel.ErrorCodeType.AlreadyProcessed:
                    return TradeErrorCode.AlreadyProcessed;
                case ResponseModel.ErrorCodeType.UnknownAsset:
                    return TradeErrorCode.UnknownAsset;
                case ResponseModel.ErrorCodeType.NoLiquidity:
                    return TradeErrorCode.NoLiquidity;
                case ResponseModel.ErrorCodeType.NotEnoughFunds:
                    return TradeErrorCode.NotEnoughFunds;
                case ResponseModel.ErrorCodeType.ReservedVolumeHigherThanBalance:
                    return TradeErrorCode.ReservedVolumeHigherThanBalance;
                case ResponseModel.ErrorCodeType.BalanceLowerThanReserved:
                    return TradeErrorCode.BalanceLowerThanReserved;
                case ResponseModel.ErrorCodeType.LeadToNegativeSpread:
                    return TradeErrorCode.LeadToNegativeSpread;
                default:
                    return TradeErrorCode.Runtime;
            }
        }
    }
}
