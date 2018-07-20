using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Indicators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models.AlgoMetaDataModels;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Services.Services
{
    [TestFixture]
    public class FunctionsServiceTest
    {
        #region FunctionsService.Initialize
        [Test]
        public void Initialize_InitialFunctionResults()
        {
            var function1 = CreateStrictFunctionMock();
            var function2 = CreateStrictFunctionMock();

            function1.SetupGet(f1 => f1.Value)
                     .Returns((double?)null);
            function2.SetupGet(f2 => f2.Value)
                     .Returns((double?)null);

            var functionService = CreateFunctionService(("Function1", function1.Object), ("Function2", function2.Object));

            functionService.Initialize();
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(null, results.GetValue("Function1"));
            Assert.AreEqual(null, results.GetValue("Function2"));

            function1.Verify();
            function2.Verify();
        }

        [Test]
        public void Initialize_DoesNotIvokeFunctions()
        {
            var function1 = CreateStrictFunctionMock();
            var function2 = CreateStrictFunctionMock();

            var functionService = CreateFunctionService(("f1", function1.Object), ("f2", function2.Object));

            functionService.Initialize();

            // Verify no to function interfaces are done during the initialization
            function1.Verify();
            function2.Verify();
        }

        #endregion

        #region FunctionService.GetCandleRequests
        [Test]
        public void GetCandleRequest_Trigered_OnMultipleFunctions()
        {
            var function1 = CreateFunctionMockByParams("USDBTC", CandleTimeInterval.Day, DateTime.UtcNow.AddDays(-1));
            var function2 = CreateFunctionMockByParams("USDSTH", CandleTimeInterval.Minute, DateTime.UtcNow.AddDays(-2));

            var functionService = CreateFunctionService(("Function1", function1.Object), ("Function2", function2.Object));
            functionService.Initialize();

            var candleRequests = functionService.GetCandleRequests("");

            AssertCandleRequest(candleRequests, ("Function1", function1), ("Function2", function2));
        }

        [Test]
        public void GetCandleRequest_Trigered_OnSingleFunction()
        {
            var function1 = CreateFunctionMockByParams("USDBTC", CandleTimeInterval.Day, DateTime.UtcNow.AddDays(-1));

            var functionService = CreateFunctionService(("Function1", function1.Object));
            functionService.Initialize();

            var candleRequests = functionService.GetCandleRequests("");

            AssertCandleRequest(candleRequests, ("Function1", function1));
        }

        [Test]
        public void GetCandleRequest_Trigered_WhenNoFunctions()
        {
            var functionService = CreateFunctionService();
            functionService.Initialize();

            var candleRequests = functionService.GetCandleRequests("");

            Assert.AreEqual(0, candleRequests.Count());
        }

        private void AssertCandleRequest(IEnumerable<CandleServiceRequest> candleRequests, params (string, Mock<IIndicator>)[] functions)
        {
            Assert.AreEqual(functions.Length, candleRequests.Count());

            foreach (var function in functions)
            {
                var functionObj = function.Item2.Object;
                var requestForFunction = candleRequests
                    .FirstOrDefault(r => r.RequestId == function.Item1);
                Assert.NotNull(requestForFunction,
                    $"Found request for function with id {function.Item1}");

                Assert.AreEqual(functionObj.AssetPair, requestForFunction.AssetPair);
                Assert.AreEqual(functionObj.CandleTimeInterval, requestForFunction.CandleInterval);
                Assert.AreEqual(functionObj.StartingDate, requestForFunction.StartFrom);
            }
        }

        #endregion

        #region FunctionService.GetFunctionResults
        [Test]
        public void GetFunctionResults_ReturnsResultsForAllFunctionResults()
        {
            var function1 = CreateStrictFunctionMock();
            function1.SetupSequence(f1 => f1.Value)
                     .Returns(1)
                     .Returns(3);
            function1.Setup(f1 => f1.WarmUp(It.IsNotNull<IList<Candle>>()))
                     .Returns(1);
            function1.Setup(f1 => f1.AddNewValue(It.IsNotNull<Candle>()))
                     .Returns(3);
            var function2 = CreateStrictFunctionMock();
            function2.SetupSequence(f2 => f2.Value)
                     .Returns(2)
                     .Returns(4);
            function2.Setup(f2 => f2.WarmUp(It.IsNotNull<IList<Candle>>()))
                     .Returns(2);
            function2.Setup(f2 => f2.AddNewValue(It.IsNotNull<Candle>()))
                     .Returns(4);

            var functionService = CreateFunctionService(("Function1", function1.Object), ("Function2", function2.Object));
            functionService.Initialize();

            // Generate some function results.
            functionService.WarmUp(CreateMultiCandlesResponse(
                "Function1",
                "Function2"));

            // Verify the warm-up results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue("Function1"));
            Assert.AreEqual(2, results.GetValue("Function2"));

            // Recalculate the functions
            var candleRequests = CreateSingleCandlesResponse(
                "Function1",
                "Function2");
            functionService.Recalculate(candleRequests);

            // Verify the results after recalculation
            var resultsAfterRecalculation = functionService.GetFunctionResults();
            Assert.AreEqual(3, resultsAfterRecalculation.GetValue("Function1"));
            Assert.AreEqual(4, resultsAfterRecalculation.GetValue("Function2"));

            // Verify function calls
            function1.Verify();
            function2.Verify();
        }

        #endregion

        #region FunctionService.WarmUp

        [Test]
        public void Warmup_Trigered_OnMultipleFunctions()
        {
            var function1 = CreateStrictFunctionMock();
            var function1WarmUpParam = new List<Candle>() { new Candle() { } };
            function1.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function1.Setup(f1 => f1.WarmUp(function1WarmUpParam))
                     .Returns(1);
            var function2 = CreateStrictFunctionMock();
            var function2WarmUpParam = new List<Candle>() { };
            function2.SetupGet(f2 => f2.Value)
                     .Returns(2);
            function2.Setup(f2 => f2.WarmUp(function2WarmUpParam))
                     .Returns(2);

            var functionService = CreateFunctionService(("Function1", function1.Object), ("Function2", function2.Object));
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(CreateMultiCandlesResponse(
                new Dictionary<string, List<Candle>>()
                {
                    {"Function1", function1WarmUpParam },
                    {"Function2", function2WarmUpParam },
                },
                "Function1",
                "Function2"));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue("Function1"));
            Assert.AreEqual(2, results.GetValue("Function2"));

            // Verify the function warm-ups are invoked with the correct parameters 
            // (function1WarmUpParam and function2WarmUpParam)
            function1.Verify();
            function2.Verify();
        }

        [Test]
        public void Warmup_Trigered_OnSingleFunction()
        {
            var function = CreateStrictFunctionMock();
            var functionWarmUpParam = new List<Candle>() { new Candle() { } };
            function.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function.Setup(f1 => f1.WarmUp(functionWarmUpParam))
                     .Returns(1);

            var functionService = CreateFunctionService(("Function1", function.Object));
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(CreateMultiCandlesResponse(
                new Dictionary<string, List<Candle>>()
                {
                    {"Function1", functionWarmUpParam }
                },
                "Function1"));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue("Function1"));

            // Verify the function warm-ups are invoked with the correct parameters 
            // (function1WarmUpParam and function2WarmUpParam)
            function.Verify();
        }

        [Test]
        public void Warmup_Trigered_WhenNoFunctions()
        {
            var functionService = CreateFunctionService();
            functionService.Initialize();

            // Warm-up.
            functionService.WarmUp(CreateMultiCandlesResponse());

            var results = functionService.GetFunctionResults();
            Assert.IsNotNull(results);
        }

        [Test]
        public void Warmup_Trigered_WhenNoCandles()
        {
            var function = CreateStrictFunctionMock();

            function.SetupGet(f => f.Value)
                     .Returns((double?)null);

            var functionService = CreateFunctionService(("Function1", function.Object));
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(new List<MultipleCandlesResponse>());

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(null, results.GetValue("Function1"));

            // Verify the function warm-ups are invoked with the correct parameters 
            function.Verify();
        }

        [Test]
        public void Warmup_Trigered_WithNullValue()
        {
            var function = CreateStrictFunctionMock();
            function.SetupGet(f => f.Value)
                     .Returns((double?)null);

            var functionService = CreateFunctionService(("Function1", function.Object));
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(null);

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(null, results.GetValue("Function1"));

            // Verify the function warm-ups are invoked with the correct parameters 
            function.Verify();
        }

        [Test]
        public void Warmup_WhenSomeFunctionDataIsMissing()
        {
            var function1 = CreateStrictFunctionMock();
            var function2 = CreateStrictFunctionMock();
            var function2WarmUpParam = new List<Candle>() { };
            function1.SetupGet(f1 => f1.Value)
                     .Returns((double?)null);
            function2.SetupGet(f2 => f2.Value)
                     .Returns(2);
            function2.Setup(f2 => f2.WarmUp(function2WarmUpParam))
                     .Returns(2);

            var functionService = CreateFunctionService(("Function1", function1.Object), ("Function2", function2.Object));
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(CreateMultiCandlesResponse(
                new Dictionary<string, List<Candle>>()
                {
                    // Do not provide function data for function1
                    {"Function2", function2WarmUpParam }
                },
                "Function2"));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(null, results.GetValue("Function1"));
            Assert.AreEqual(2, results.GetValue("Function2"));

            // Verify the function warm-ups are invoked with the correct parameters 
            // (function1WarmUpParam and function2WarmUpParam)
            function1.Verify();
            function2.Verify();
        }

        [Test]
        public void Warmup_WhenExcessFunctionDataIsProvided()
        {
            var function1 = CreateStrictFunctionMock();
            var function1WarmUpParam = new List<Candle>() { new Candle() { } };
            function1.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function1.Setup(f1 => f1.WarmUp(function1WarmUpParam))
                     .Returns(1);
            var function2 = CreateStrictFunctionMock();
            var function2WarmUpParam = new List<Candle>() { };
            function2.SetupGet(f2 => f2.Value)
                     .Returns(2);
            function2.Setup(f2 => f2.WarmUp(function2WarmUpParam))
                     .Returns(2);

            var functionService = CreateFunctionService(("Function1", function1.Object), ("Function2", function2.Object));
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(CreateMultiCandlesResponse(
                new Dictionary<string, List<Candle>>()
                {
                    {"NON_EXISTING_FUNCTION", new List<Candle>(){ new Candle() } },
                    {"Function1", function1WarmUpParam },
                    {"NON_EXISTING_FUNCTION_1", new List<Candle>(){ new Candle() } },
                    {"Function2", function2WarmUpParam },
                    {"NON_EXISTING_FUNCTION_2", new List<Candle>(){ new Candle() } },
                },
                "NON_EXISTING_FUNCTION",
                "Function1",
                "NON_EXISTING_FUNCTION_1",
                "Function2",
                "NON_EXISTING_FUNCTION_2"));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue("Function1"));
            Assert.AreEqual(2, results.GetValue("Function2"));

            // Verify the function warm-ups are invoked with the correct parameters 
            // (function1WarmUpParam and function2WarmUpParam)
            function1.Verify();
            function2.Verify();
        }

        private IList<MultipleCandlesResponse> CreateMultiCandlesResponse(
            Dictionary<string, List<Candle>> functioIdToCandlesMap, params string[] functionIds)
        {
            var result = new List<MultipleCandlesResponse>();

            foreach (var functionId in functionIds)
            {
                result.Add(new MultipleCandlesResponse()
                {
                    RequestId = functionId,
                    Candles = functioIdToCandlesMap[functionId]
                });
            }

            return result;
        }

        #endregion

        #region FunctionsService.Recalculate

        [Test]
        public void Recalculate_Trigered_OnMultipleFunctions()
        {
            var function1 = CreateStrictFunctionMock();
            var function1RecalculateParam = new Candle() { };
            function1.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function1.Setup(f1 => f1.AddNewValue(function1RecalculateParam))
                     .Returns(1);
            var function2 = CreateStrictFunctionMock();
            var function2RecalculateParam = new Candle() { };
            function2.SetupGet(f2 => f2.Value)
                     .Returns(2);
            function2.Setup(f2 => f2.AddNewValue(function2RecalculateParam))
                     .Returns(2);

            var functionService = CreateFunctionService(("Function1", function1.Object), ("Function2", function2.Object));
            functionService.Initialize();

            // Warm-up
            functionService.Recalculate(CreateSingleCandlesResponse(
                new Dictionary<string, Candle>()
                {
                    {"Function1", function1RecalculateParam },
                    {"Function2", function2RecalculateParam },
                },
                "Function1",
                "Function2"));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue("Function1"));
            Assert.AreEqual(2, results.GetValue("Function2"));

            // Verify the function warm-ups are invoked with the correct parameters
            function1.Verify();
            function2.Verify();
        }


        [Test]
        public void Recalculate_Trigered_OnSingleFunction()
        {
            var function1 = CreateStrictFunctionMock();
            var function1RecalculateParam = new Candle() { };
            function1.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function1.Setup(f1 => f1.AddNewValue(function1RecalculateParam))
                     .Returns(1);

            var functionService = CreateFunctionService(("Function1", function1.Object));
            functionService.Initialize();

            // Warm-up
            functionService.Recalculate(CreateSingleCandlesResponse(
                new Dictionary<string, Candle>()
                {
                    {"Function1", function1RecalculateParam },
                },
                "Function1"));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue("Function1"));

            // Verify the function warm-ups are invoked with the correct parameters
            function1.Verify();
        }

        [Test]
        public void Recalculate_Trigered_WhenNoFunctions()
        {
            var functionService = CreateFunctionService();
            functionService.Initialize();

            // Recalculate
            functionService.Recalculate(CreateSingleCandlesResponse());

            var results = functionService.GetFunctionResults();
            Assert.IsNotNull(results);
        }

        [Test]
        public void Recalculate_Trigered_WhenNoCandles()
        {
            var function = CreateStrictFunctionMock();
            var functionWarmUpParam = new List<Candle>() { new Candle() { } };
            function.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function.Setup(f1 => f1.WarmUp(functionWarmUpParam))
                     .Returns(1);

            var functionService = CreateFunctionService(("Function1", function.Object));
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(CreateMultiCandlesResponse(
                new Dictionary<string, List<Candle>>()
                {
                    {"Function1", functionWarmUpParam }
                },
                "Function1"));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue("Function1"));

            functionService.Recalculate(new List<SingleCandleResponse>());

            // Assert function results
            var resultsAfterRecalculate = functionService.GetFunctionResults();
            Assert.AreEqual(1, resultsAfterRecalculate.GetValue("Function1"));

            // Verify the function warm-ups are invoked with the correct parameters
            function.Verify();
        }

        [Test]
        public void Recalculate_Trigered_WhenNoCandles_AndNoWarmUp()
        {
            var function = CreateStrictFunctionMock();

            function.SetupGet(f => f.Value)
                    .Returns((double?)null);

            var functionService = CreateFunctionService(("Function1", function.Object));
            functionService.Initialize();

            // Recalculate
            functionService.Recalculate(new List<SingleCandleResponse>());

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(null, results.GetValue("Function1"));

            // Verify the function warm-ups are invoked with the correct parameters 
            function.Verify();
        }

        [Test]
        public void Recalculate_Trigered_WithNullValue()
        {
            var function1 = CreateStrictFunctionMock();
            var function1WarmUpParam = new List<Candle>() { new Candle() { } };
            function1.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function1.Setup(f1 => f1.WarmUp(function1WarmUpParam))
                     .Returns(1);
            var function2 = CreateStrictFunctionMock();
            var function2WarmUpParam = new List<Candle>() { };
            function2.SetupGet(f2 => f2.Value)
                     .Returns(2);
            function2.Setup(f2 => f2.WarmUp(function2WarmUpParam))
                     .Returns(2);

            var functionService = CreateFunctionService(("Function1", function1.Object), ("Function2", function2.Object));
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(CreateMultiCandlesResponse(
                new Dictionary<string, List<Candle>>()
                {
                    {"Function1", function1WarmUpParam },
                    {"Function2", function2WarmUpParam },
                },
                "Function1",
                "Function2"));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue("Function1"));
            Assert.AreEqual(2, results.GetValue("Function2"));

            functionService.Recalculate(null);

            var resultsAfterRecalculate = functionService.GetFunctionResults();
            Assert.AreEqual(1, resultsAfterRecalculate.GetValue("Function1"));
            Assert.AreEqual(2, resultsAfterRecalculate.GetValue("Function2"));

            // Verify the function warm-ups are invoked with the correct parameters 
            // (function1WarmUpParam and function2WarmUpParam)
            function1.Verify();
            function2.Verify();
        }

        [Test]
        public void Recalculate_Trigered_WithNullValue_AndNoWarmUp()
        {
            var function = CreateStrictFunctionMock();

            function.SetupGet(f1 => f1.Value)
                     .Returns((double?)null);

            var functionService = CreateFunctionService(("Function1", function.Object));
            functionService.Initialize();

            // Warm-up
            functionService.Recalculate(null);

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(null, results.GetValue("Function1"));

            // Verify the function warm-ups are invoked with the correct parameters 
            function.Verify();
        }

        [Test]
        public void Recalculate_WhenSomeFunctionDataIsMissing()
        {
            var function1 = CreateStrictFunctionMock();
            var function2 = CreateStrictFunctionMock();
            var function2RecalculateParam = new Candle() { };
            function1.SetupGet(f1 => f1.Value)
                     .Returns((double?)null);
            function2.Setup(f2 => f2.AddNewValue(function2RecalculateParam))
                     .Returns(2);
            function2.SetupGet(f2 => f2.Value)
                     .Returns(2);

            var functionService = CreateFunctionService(("Function1", function1.Object), ("Function2", function2.Object));
            functionService.Initialize();

            // Warm-up
            functionService.Recalculate(CreateSingleCandlesResponse(
                new Dictionary<string, Candle>()
                {
                    // Do not provide function data for function1
                    {"Function2", function2RecalculateParam }
                },
                "Function2"));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(null, results.GetValue("Function1"));
            Assert.AreEqual(2, results.GetValue("Function2"));

            // Verify the function warm-ups are invoked with the correct parameters 
            // (function1WarmUpParam and function2WarmUpParam)
            function1.Verify();
            function2.Verify();
        }

        [Test]
        public void Recalculate_WhenExcessFunctionDataIsProvided()
        {
            var function1 = CreateStrictFunctionMock();
            var function1RecalculateParam = new Candle();
            function1.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function1.Setup(f1 => f1.AddNewValue(function1RecalculateParam))
                     .Returns(1);
            var function2 = CreateStrictFunctionMock();
            var function2RecalculateParam = new Candle();
            function2.SetupGet(f2 => f2.Value)
                     .Returns(2);
            function2.Setup(f2 => f2.AddNewValue(function2RecalculateParam))
                     .Returns(2);

            var functionService = CreateFunctionService(("Function1", function1.Object), ("Function2", function2.Object));
            functionService.Initialize();

            // Warm-up
            functionService.Recalculate(CreateSingleCandlesResponse(
                new Dictionary<string, Candle>()
                {
                    {"NON_EXISTING_FUNCTION",  new Candle()},
                    {"Function1", function1RecalculateParam },
                    {"NON_EXISTING_FUNCTION_1", new Candle() },
                    {"Function2", function2RecalculateParam },
                    {"NON_EXISTING_FUNCTION_2", new Candle()  },
                },
                "NON_EXISTING_FUNCTION",
                "Function1",
                "NON_EXISTING_FUNCTION_1",
                "Function2",
                "NON_EXISTING_FUNCTION_2"));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue("Function1"));
            Assert.AreEqual(2, results.GetValue("Function2"));

            // Verify the function warm-ups are invoked with the correct parameters 
            // (function1WarmUpParam and function2WarmUpParam)
            function1.Verify();
            function2.Verify();
        }

        private IList<SingleCandleResponse> CreateSingleCandlesResponse(
            Dictionary<string, Candle> functioIdToCandlesMap, params string[] functionIds)
        {
            var result = new List<SingleCandleResponse>();

            foreach (var functionId in functionIds)
            {
                result.Add(new SingleCandleResponse()
                {
                    RequestId = functionId,
                    Candle = functioIdToCandlesMap[functionId]
                });
            }

            return result;
        }
        #endregion

        private IAlgoSettingsService GetIAlgoSettingsServiceMockReturnCorrectData()
        {
            var service = new Mock<IAlgoSettingsService>();

            service.Setup(f => f.GetAlgoInstance()).Returns(new Models.Models.AlgoClientInstanceData()
            {
                AlgoMetaDataInformation = new AlgoMetaDataInformation()
                {
                    Functions = new List<AlgoMetaDataFunction>()
                    {
                        new AlgoMetaDataFunction()
                        {
                            Id="SAM_TEST",
                            Type = "Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Functions.SMA.SmaFunction",
                            FunctionParameterType = "Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Functions.SMA.SmaParameters",
                            Parameters = new List<AlgoMetaDataParameter>()
                                {
                                    new AlgoMetaDataParameter()
                                    {
                                        Key="FunctionInstanceIdentifier",
                                        Value="SAM_TEST",
                                        Type= "String"
                                    },
                                     new AlgoMetaDataParameter()
                                    {
                                        Key="StartingDate",
                                        Value="2018-02-10",
                                        Type= "DateTime"
                                    },
                                    new AlgoMetaDataParameter()
                                    {
                                        Key="Capacity",
                                        Value="10",
                                        Type= "int"
                                    },
                                    new AlgoMetaDataParameter()
                                    {
                                        Key="AssetPair",
                                        Value="BTCEUR",
                                        Type= "String"
                                    }
                                }
                        }
                    }

                }
            });

            return service.Object;
        }

        private IAlgoSettingsService GetIAlgoSettingsServiceMock_ReturnNoAlgoInstance()
        {
            var service = new Mock<IAlgoSettingsService>();
            service.Setup(f => f.GetAlgoInstance()).Returns<AlgoClientInstanceData>(null);
            return service.Object;
        }

        private FunctionsService CreateFunctionService(params (string, IIndicator)[] functions)
        {
            var fService = new FunctionsService(Mock.Of<IAlgoSettingsService>(
                s => s.GetAlgoInstance() == new AlgoClientInstanceData
                {
                    AlgoMetaDataInformation = new AlgoMetaDataInformation
                    {
                        Functions = new List<AlgoMetaDataFunction>()
                    }
                }));

            foreach(var indicator in functions)
                fService.RegisterIndicator(indicator.Item1, indicator.Item2);

            return fService;
        }

        private Mock<IIndicator> CreateStrictFunctionMock()
        {
            var result = new Mock<IIndicator>(MockBehavior.Strict);

            result.SetupGet(r => r.StartingDate).Returns(default(DateTime));
            result.SetupGet(r => r.EndingDate).Returns(default(DateTime));

            return result;
        }

        private Mock<IIndicator> CreateFunctionMockByParams(
            string assetPair,
            CandleTimeInterval candleTimeInterval,
            DateTime startFrom)
        {
            var result = new Mock<IIndicator>(MockBehavior.Strict);
            result.SetupGet(f => f.AssetPair).Returns(assetPair);
            result.SetupGet(f => f.CandleTimeInterval).Returns(candleTimeInterval);
            result.SetupGet(f => f.StartingDate).Returns(startFrom);
            result.SetupGet(f => f.EndingDate).Returns(default(DateTime));
            return result;
        }

        private IList<MultipleCandlesResponse> CreateMultiCandlesResponse(params string[] functionIds)
        {
            var result = new List<MultipleCandlesResponse>();

            foreach (var functionId in functionIds)
            {
                result.Add(new MultipleCandlesResponse()
                {
                    RequestId = functionId,
                    Candles = new List<Candle>()
                    {
                        new Candle() {Open = 10, Close = 20, Low = 9, High = 21, DateTime = DateTime.UtcNow.AddDays(-1), LastTradePrice = 19},
                        new Candle() {Open = 20, Close = 30, Low = 19, High = 31, DateTime = DateTime.UtcNow.AddDays(-2), LastTradePrice = 19}
                    }
                });
            }

            return result;
        }

        private IList<SingleCandleResponse> CreateSingleCandlesResponse(params string[] functionIds)
        {
            var result = new List<SingleCandleResponse>();

            foreach (var functionId in functionIds)
            {
                result.Add(new SingleCandleResponse()
                {
                    RequestId = functionId,
                    Candle = new Candle() { Open = 10, Close = 20, Low = 9, High = 21, DateTime = DateTime.UtcNow.AddDays(-1), LastTradePrice = 19 }
                });
            }

            return result;
        }
    }
}
