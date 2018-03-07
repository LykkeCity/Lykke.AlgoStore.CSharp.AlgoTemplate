using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IFunctionInitializationService"/> implementation providing the services
    /// required to run the algo
    /// </summary>
    public class FunctionInitializationService : IFunctionInitializationService
    {
        private IAlgoSettingsService _algoSettingsService;

        public FunctionInitializationService(IAlgoSettingsService algoSettingsService)
        {
            _algoSettingsService = algoSettingsService;
        }

        public IList<IFunction> GetAllFunctions()
        {
            var algoInstance = _algoSettingsService.GetAlgoInstance();

            if (algoInstance == null)
                return null;

            IList<IFunction> functions = new List<IFunction>();

            foreach (var function in algoInstance.AlgoMetaDataInformation.Functions)
            {
                Type functionType = Type.GetType(function.Type);

                string functionParamTypeName = function.FunctionParameterType;
                FunctionParamsBase paramObject = new FunctionParamsBase();

                Type parameterType = Type.GetType(functionParamTypeName);
                paramObject = (FunctionParamsBase)Activator.CreateInstance(parameterType);

                foreach (var param in function.Parameters)
                {
                    PropertyInfo prop = parameterType.GetProperty(param.Key);
                    if (prop != null && prop.CanWrite)
                    {
                        if (prop.PropertyType.IsEnum)
                            prop.SetValue(paramObject, Enum.ToObject(prop.PropertyType, Convert.ToInt32(param.Value)), null);
                        else
                            prop.SetValue(paramObject, Convert.ChangeType(param.Value, prop.PropertyType), null);
                    }
                }

                var funcObj = Activator.CreateInstance(functionType, paramObject);
                functions.Add((IFunction)funcObj);
            }

            return functions;
        }
    }
}
