namespace Lykke.AlgoStore.Algo
{
    public abstract class ServiceResponse<R, E>
    {
        public R Result { get; set; }
        public ErrorModel<E> Error { get; set; }

        public abstract bool IsSuccess();
    }
}
