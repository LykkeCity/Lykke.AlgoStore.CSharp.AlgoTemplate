using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions
{
    public static class ErrorCodeTypeExtensions
    {
        public static TradeErrorCode ToTradeErrorCode(this ResponseModel.ErrorModel meaErrorCodeType)
        {
            switch (meaErrorCodeType.Code)
            {
                case ErrorCodeType.LowBalance:
                    return TradeErrorCode.LowBalance;
                case ErrorCodeType.AlreadyProcessed:
                    return TradeErrorCode.AlreadyProcessed;
                case ErrorCodeType.UnknownAsset:
                    return TradeErrorCode.UnknownAsset;
                case ErrorCodeType.NoLiquidity:
                    return TradeErrorCode.NoLiquidity;
                case ErrorCodeType.NotEnoughFunds:
                    return TradeErrorCode.NotEnoughFunds;
                case ErrorCodeType.Dust:
                    return TradeErrorCode.Dust;
                case ErrorCodeType.ReservedVolumeHigherThanBalance:
                    return TradeErrorCode.ReservedVolumeHigherThanBalance;
                case ErrorCodeType.BalanceLowerThanReserved:
                    return TradeErrorCode.BalanceLowerThanReserved;
                case ErrorCodeType.LeadToNegativeSpread:
                    return TradeErrorCode.LeadToNegativeSpread;
                case ErrorCodeType.NotFound:
                    return TradeErrorCode.NotFound;
                case ErrorCodeType.Runtime:
                    return TradeErrorCode.Runtime;
                case ErrorCodeType.NotFoundPrevious:
                    return TradeErrorCode.NotFoundPrevious;
                case ErrorCodeType.Replaced:
                    return TradeErrorCode.Replaced;
                case ErrorCodeType.InvalidPrice:
                    return TradeErrorCode.InvalidPrice;
                case ErrorCodeType.Duplicate:
                    return TradeErrorCode.Duplicate;
                case ErrorCodeType.InvalidFee:
                    return TradeErrorCode.InvalidFee;
                case ErrorCodeType.BadRequest:
                    return TradeErrorCode.BadRequest;
                case ErrorCodeType.InvalidInputField:
                    return TradeErrorCode.InvalidInputField;
                default:
                    return TradeErrorCode.Runtime;
            }
        }
    }
}
