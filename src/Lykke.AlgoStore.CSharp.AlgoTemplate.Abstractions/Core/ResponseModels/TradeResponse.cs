using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.ResponseModels.ErrorCodes;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.ResponseModels
{
    public class TradeResponse : ServiceResponse<double, TradeErrorCode>
    {
        public override bool IsSuccess()
        {
            return Error == null && Result > 0;
        }

        private TradeResponse()
        {
        }

        public static TradeResponse CreateOk(double result)
        {
            return new TradeResponse
            {
                Result = result
            };
        }

        public static TradeResponse CreateFail(TradeErrorCode errorCodeType, string message = null)
        {
            return new TradeResponse
            {
                Error = new ErrorModel<TradeErrorCode>()
                {
                    ErrorCode = errorCodeType,
                    Message = message
                }
            };
        }

    }
}
