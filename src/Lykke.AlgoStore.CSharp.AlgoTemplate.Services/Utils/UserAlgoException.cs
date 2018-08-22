using System;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    public class UserAlgoException : Exception
    {
        public UserAlgoException(Exception innerException) 
            : base("Error occured during an algo event", innerException)
        {
        }
    }
}
