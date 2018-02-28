using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.CSharp.Algo.Core.Domain
{
    public interface IFunctionsParameters
    {
        object GetParameterValue(string parameterName);
    }
}
