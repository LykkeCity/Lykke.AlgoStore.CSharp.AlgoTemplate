using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.SMA;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IFunctionInitializationService"/> implementation providing the services
    /// required to run the algo
    /// </summary>
    public class FunctionInitializationService : IFunctionInitializationService
    {
        private IAlgoClientInstanceRepository _algoClientInstanceRepository;

        private IAlgoSettingsService _algoSettingsService;

        public FunctionInitializationService(IAlgoClientInstanceRepository algoClientInstanceRepository,
            IAlgoSettingsService algoSettingsService)
        {
            _algoClientInstanceRepository = algoClientInstanceRepository;
            _algoSettingsService = algoSettingsService;
        }

        public IList<IFunction> GetAllFunctions()
        {
            var algoInstance = _algoSettingsService.GetAlgoInstance();

            IList<IFunction> functions = new List<IFunction>();

            foreach (var function in algoInstance.AlgoMetaDataInformation.Functions)
            {
                //function.Id == FunctionInstanceIdentifier
                Type functionType = Type.GetType(function.Type);

                Type parameterType = null;
                string functionParamTypeName = string.Empty;
                FunctionParamsBase paramObject = new FunctionParamsBase();

                foreach (var param in function.Parameters)
                {
                    if (functionParamTypeName != param.ParameterType)
                    {
                        functionParamTypeName = param.ParameterType;
                        parameterType = Type.GetType(param.ParameterType);
                        paramObject = (FunctionParamsBase)Activator.CreateInstance(parameterType);
                    }

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
