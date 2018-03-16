using Lykke.AlgoStore.CSharp.Algo.Core.Functions;
using Lykke.AlgoStore.CSharp.Algo.Implemention;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Services.Services
{
    public class WorkflowServiceTest
    {
        private readonly string _tradedAsset = "USD";
        private readonly string _assetPair = "BTCEUR";
        private readonly double _volume = 0.2;
        private readonly DateTime _startFrom = new DateTime(2018, 2, 10);

        [Test]
        public void GetCorrect_ParametersMetadata()
        {
            var algoInstance = GetAlgoInstanceMockReturnCorrectData();
            var algo = new DummyAlgo();

            WorkflowService service = new WorkflowService(null, null, null, null, null, null, null, null, algo);

            service.SetUpAlgoParameters(algoInstance);
            Assert.AreEqual(_tradedAsset, algo.TradedAsset);
            Assert.AreEqual(_assetPair, algo.AssetPair);
            Assert.AreEqual(_volume, algo.Volume);
            Assert.AreEqual(_startFrom, algo.StartFrom);
        }

        [Test]
        public void Get_NoParametersMetadata()
        {
            var algo = new DummyAlgo();

            WorkflowService service = new WorkflowService(null, null, null, null, null, null, null, null, algo);

            service.SetUpAlgoParameters(null);
            Assert.IsNull(algo.TradedAsset);
            Assert.IsNull(algo.AssetPair);
        }

        private static void Then_Data_ShouldBe_Equal(IList<IFunction> first, IList<IFunction> second)
        {
            string serializedFirst = JsonConvert.SerializeObject(first);
            string serializedSecond = JsonConvert.SerializeObject(second);
            Assert.AreEqual(serializedFirst, serializedSecond);
        }

        #region MockUps

        private AlgoClientInstanceData GetAlgoInstanceMockReturnCorrectData()
        {
            var result = new Models.Models.AlgoClientInstanceData()
            {
                AlgoMetaDataInformation = new AlgoMetaDataInformation()
                {
                    Parameters = new List<AlgoMetaDataParameter>()
                    {

                        new AlgoMetaDataParameter()
                        {
                            Key = "TradedAsset",
                            Value = _tradedAsset,
                            Type = "String"
                        },
                         new AlgoMetaDataParameter()
                        {
                            Key = "StartFrom",
                            Value = _startFrom.ToString(),
                            Type = "DateTime"
                        },
                        new AlgoMetaDataParameter()
                        {
                            Key = "Volume",
                            Value = _volume.ToString(),
                            Type = "double"
                        },
                        new AlgoMetaDataParameter()
                        {
                            Key = "AssetPair",
                            Value = _assetPair,
                            Type = "String"
                        }
                    }
                }
            };
            return result;
        }

        #endregion
    }
}
