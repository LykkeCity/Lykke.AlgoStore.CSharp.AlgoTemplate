namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.ResponseModels
{
    public class ErrorModel<T>
    {
        public T ErrorCode;
        public string Message;
    }
}
