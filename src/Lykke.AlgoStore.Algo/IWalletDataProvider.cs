using System.Collections.Generic;

namespace Lykke.AlgoStore.Algo
{
    public interface IWalletDataProvider
    {
        Dictionary<string, WalletBalance> GetBalances();
    }
}
