using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;
using System.Collections.Generic;
using System.Reflection;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;

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

            IList<IFunction> functions = new List<IFunction>();

            if (algoInstance == null || algoInstance.AlgoMetaDataInformation.Functions == null)
                return functions;

            foreach (var function in algoInstance.AlgoMetaDataInformation.Functions)
            {
                //All functions should have parameters - defined in one class, if parameters are null, function can not be instantiated.
                if (function.Parameters == null)
                    continue;

                //Load old (obsolete) and new assembly that holds functions and decide which one to use
                var oldAssembly = Assembly.Load("Lykke.AlgoStore.CSharp.AlgoTemplate.Services");
                var newAssembly = Assembly.Load("Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions");

                var assembly =
                    oldAssembly.GetType(function.Type) == null ||
                    oldAssembly.GetType(function.FunctionParameterType) == null
                        ? newAssembly
                        : oldAssembly;

                var functionType = assembly.GetType(function.Type);
                var parameterType = assembly.GetType(function.FunctionParameterType);
                var paramObject = (FunctionParamsBase)Activator.CreateInstance(parameterType);

                foreach (var param in function.Parameters)
                {
                    var prop = parameterType.GetProperty(param.Key);
                    if (prop != null && prop.CanWrite)
                    {
                        if (prop.PropertyType.IsEnum)
                            prop.SetValue(paramObject, Enum.ToObject(prop.PropertyType, Convert.ToInt32(param.Value)), null);
                        else
                            prop.SetValue(paramObject, Convert.ChangeType(param.Value, prop.PropertyType), null);
                    }
                }

                var functionObject = (IFunction)Activator.CreateInstance(functionType, paramObject);
                //functionObject.FunctionParameters.FunctionInstanceIdentifier = function.Id;
                functions.Add(functionObject);
            }

            return functions;
        }
    }
}
