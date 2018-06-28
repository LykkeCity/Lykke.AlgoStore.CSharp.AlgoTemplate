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
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Functions;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Functions.SMA;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Services.Services
{
    [TestFixture]
    public class FunctionsServiceTest
    {
        #region FunctionsService.Initialize
        [Test]
        public void Initialize_InitialFunctionResults()
        {
            var function1 = CreateStrictFunctionMock("Function1");
            var function2 = CreateStrictFunctionMock("Function2");

            function1.SetupGet(f1 => f1.Value)
                     .Returns((double?)null);
            function2.SetupGet(f2 => f2.Value)
                     .Returns((double?)null);

            var functionService = CreateFunctionService(function1.Object, function2.Object);

            functionService.Initialize();
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(null, results.GetValue(function1.Object.FunctionParameters.FunctionInstanceIdentifier));
            Assert.AreEqual(null, results.GetValue(function2.Object.FunctionParameters.FunctionInstanceIdentifier));

            function1.Verify();
            function2.Verify();
        }

        [Test]
        public void Initialize_DoesNotIvokeFunctions()
        {
            var function1 = CreateStrictFunctionMock("Function1");
            var function2 = CreateStrictFunctionMock("Function2");

            var functionService = CreateFunctionService(function1.Object, function2.Object);

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
            var function1 = CreateFunctionMockByParams("Function1", "USDBTC", CandleTimeInterval.Day, DateTime.UtcNow.AddDays(-1));
            var function2 = CreateFunctionMockByParams("Function2", "USDSTH", CandleTimeInterval.Minute, DateTime.UtcNow.AddDays(-2));

            var functionService = CreateFunctionService(function1.Object, function2.Object);
            functionService.Initialize();

            var candleRequests = functionService.GetCandleRequests("");

            AssertCandleRequest(candleRequests, function1, function2);
        }

        [Test]
        public void GetCandleRequest_Trigered_OnSingleFunction()
        {
            var function1 = CreateFunctionMockByParams("Function1", "USDBTC", CandleTimeInterval.Day, DateTime.UtcNow.AddDays(-1));

            var functionService = CreateFunctionService(function1.Object);
            functionService.Initialize();

            var candleRequests = functionService.GetCandleRequests("");

            AssertCandleRequest(candleRequests, function1);
        }

        [Test]
        public void GetCandleRequest_Trigered_WhenNoFunctions()
        {
            var functionService = CreateFunctionService();
            functionService.Initialize();

            var candleRequests = functionService.GetCandleRequests("");

            Assert.AreEqual(0, candleRequests.Count());
        }

        private void AssertCandleRequest(IEnumerable<CandleServiceRequest> candleRequests, params Mock<IFunction>[] functions)
        {
            Assert.AreEqual(functions.Length, candleRequests.Count());

            foreach (var function in functions)
            {
                var functionParams = function.Object.FunctionParameters;
                var requestForFunction = candleRequests
                    .FirstOrDefault(r => r.RequestId == functionParams.FunctionInstanceIdentifier);
                Assert.NotNull(requestForFunction,
                    $"Found request for function with id {functionParams.FunctionInstanceIdentifier}");

                Assert.AreEqual(functionParams.FunctionInstanceIdentifier, requestForFunction.RequestId);
                Assert.AreEqual(functionParams.AssetPair, requestForFunction.AssetPair);
                Assert.AreEqual(functionParams.CandleTimeInterval, requestForFunction.CandleInterval);
                Assert.AreEqual(functionParams.StartingDate, requestForFunction.StartFrom);
            }
        }

        #endregion

        #region FunctionService.GetFunctionResults
        [Test]
        public void GetFunctionResults_ReturnsResultsForAllFunctionResults()
        {
            var function1 = CreateStrictFunctionMock("Function1");
            function1.SetupSequence(f1 => f1.Value)
                     .Returns(1)
                     .Returns(3);
            function1.Setup(f1 => f1.WarmUp(It.IsNotNull<IList<Candle>>()))
                     .Returns(1);
            function1.Setup(f1 => f1.AddNewValue(It.IsNotNull<Candle>()))
                     .Returns(3);
            var function2 = CreateStrictFunctionMock("Function2");
            function2.SetupSequence(f2 => f2.Value)
                     .Returns(2)
                     .Returns(4);
            function2.Setup(f2 => f2.WarmUp(It.IsNotNull<IList<Candle>>()))
                     .Returns(2);
            function2.Setup(f2 => f2.AddNewValue(It.IsNotNull<Candle>()))
                     .Returns(4);

            var functionService = CreateFunctionService(function1.Object, function2.Object);
            functionService.Initialize();

            // Generate some function results.
            functionService.WarmUp(CreateMultiCandlesResponse(
                function1.Object.FunctionParameters.FunctionInstanceIdentifier,
                function2.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Verify the warm-up results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue(function1.Object.FunctionParameters.FunctionInstanceIdentifier));
            Assert.AreEqual(2, results.GetValue(function2.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Recalculate the functions
            var candleRequests = CreateSingleCandlesResponse(
                function1.Object.FunctionParameters.FunctionInstanceIdentifier,
                function2.Object.FunctionParameters.FunctionInstanceIdentifier);
            functionService.Recalculate(candleRequests);

            // Verify the results after recalculation
            var resultsAfterRecalculation = functionService.GetFunctionResults();
            Assert.AreEqual(3, resultsAfterRecalculation.GetValue(function1.Object.FunctionParameters.FunctionInstanceIdentifier));
            Assert.AreEqual(4, resultsAfterRecalculation.GetValue(function2.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Verify function calls
            function1.Verify();
            function2.Verify();
        }

        #endregion

        #region FunctionService.WarmUp

        [Test]
        public void Warmup_Trigered_OnMultipleFunctions()
        {
            var function1 = CreateStrictFunctionMock("Function1");
            var function1WarmUpParam = new List<Candle>() { new Candle() { } };
            function1.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function1.Setup(f1 => f1.WarmUp(function1WarmUpParam))
                     .Returns(1);
            var function2 = CreateStrictFunctionMock("Function2");
            var function2WarmUpParam = new List<Candle>() { };
            function2.SetupGet(f2 => f2.Value)
                     .Returns(2);
            function2.Setup(f2 => f2.WarmUp(function2WarmUpParam))
                     .Returns(2);

            var functionService = CreateFunctionService(function1.Object, function2.Object);
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(CreateMultiCandlesResponse(
                new Dictionary<string, List<Candle>>()
                {
                    {function1.Object.FunctionParameters.FunctionInstanceIdentifier, function1WarmUpParam },
                    {function2.Object.FunctionParameters.FunctionInstanceIdentifier, function2WarmUpParam },
                },
                function1.Object.FunctionParameters.FunctionInstanceIdentifier,
                function2.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue(function1.Object.FunctionParameters.FunctionInstanceIdentifier));
            Assert.AreEqual(2, results.GetValue(function2.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Verify the function warm-ups are invoked with the correct parameters 
            // (function1WarmUpParam and function2WarmUpParam)
            function1.Verify();
            function2.Verify();
        }

        [Test]
        public void Warmup_Trigered_OnSingleFunction()
        {
            var function = CreateStrictFunctionMock("Function1");
            var functionWarmUpParam = new List<Candle>() { new Candle() { } };
            function.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function.Setup(f1 => f1.WarmUp(functionWarmUpParam))
                     .Returns(1);

            var functionService = CreateFunctionService(function.Object);
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(CreateMultiCandlesResponse(
                new Dictionary<string, List<Candle>>()
                {
                    {function.Object.FunctionParameters.FunctionInstanceIdentifier, functionWarmUpParam }
                },
                function.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue(function.Object.FunctionParameters.FunctionInstanceIdentifier));

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
            var function = CreateStrictFunctionMock("Function1");

            function.SetupGet(f => f.Value)
                     .Returns((double?)null);

            var functionService = CreateFunctionService(function.Object);
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(new List<MultipleCandlesResponse>());

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(null, results.GetValue(function.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Verify the function warm-ups are invoked with the correct parameters 
            function.Verify();
        }

        [Test]
        public void Warmup_Trigered_WithNullValue()
        {
            var function = CreateStrictFunctionMock("Function1");
            function.SetupGet(f => f.Value)
                     .Returns((double?)null);

            var functionService = CreateFunctionService(function.Object);
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(null);

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(null, results.GetValue(function.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Verify the function warm-ups are invoked with the correct parameters 
            function.Verify();
        }

        [Test]
        public void Warmup_WhenSomeFunctionDataIsMissing()
        {
            var function1 = CreateStrictFunctionMock("Function1");
            var function2 = CreateStrictFunctionMock("Function2");
            var function2WarmUpParam = new List<Candle>() { };
            function1.SetupGet(f1 => f1.Value)
                     .Returns((double?)null);
            function2.SetupGet(f2 => f2.Value)
                     .Returns(2);
            function2.Setup(f2 => f2.WarmUp(function2WarmUpParam))
                     .Returns(2);

            var functionService = CreateFunctionService(function1.Object, function2.Object);
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(CreateMultiCandlesResponse(
                new Dictionary<string, List<Candle>>()
                {
                    // Do not provide function data for function1
                    {function2.Object.FunctionParameters.FunctionInstanceIdentifier, function2WarmUpParam }
                },
                function2.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(null, results.GetValue(function1.Object.FunctionParameters.FunctionInstanceIdentifier));
            Assert.AreEqual(2, results.GetValue(function2.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Verify the function warm-ups are invoked with the correct parameters 
            // (function1WarmUpParam and function2WarmUpParam)
            function1.Verify();
            function2.Verify();
        }

        [Test]
        public void Warmup_WhenExcessFunctionDataIsProvided()
        {
            var function1 = CreateStrictFunctionMock("Function1");
            var function1WarmUpParam = new List<Candle>() { new Candle() { } };
            function1.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function1.Setup(f1 => f1.WarmUp(function1WarmUpParam))
                     .Returns(1);
            var function2 = CreateStrictFunctionMock("Function2");
            var function2WarmUpParam = new List<Candle>() { };
            function2.SetupGet(f2 => f2.Value)
                     .Returns(2);
            function2.Setup(f2 => f2.WarmUp(function2WarmUpParam))
                     .Returns(2);

            var functionService = CreateFunctionService(function1.Object, function2.Object);
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(CreateMultiCandlesResponse(
                new Dictionary<string, List<Candle>>()
                {
                    {"NON_EXISTING_FUNCTION", new List<Candle>(){ new Candle() } },
                    {function1.Object.FunctionParameters.FunctionInstanceIdentifier, function1WarmUpParam },
                    {"NON_EXISTING_FUNCTION_1", new List<Candle>(){ new Candle() } },
                    {function2.Object.FunctionParameters.FunctionInstanceIdentifier, function2WarmUpParam },
                    {"NON_EXISTING_FUNCTION_2", new List<Candle>(){ new Candle() } },
                },
                "NON_EXISTING_FUNCTION",
                function1.Object.FunctionParameters.FunctionInstanceIdentifier,
                "NON_EXISTING_FUNCTION_1",
                function2.Object.FunctionParameters.FunctionInstanceIdentifier,
                "NON_EXISTING_FUNCTION_2"));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue(function1.Object.FunctionParameters.FunctionInstanceIdentifier));
            Assert.AreEqual(2, results.GetValue(function2.Object.FunctionParameters.FunctionInstanceIdentifier));

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
            var function1 = CreateStrictFunctionMock("Function1");
            var function1RecalculateParam = new Candle() { };
            function1.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function1.Setup(f1 => f1.AddNewValue(function1RecalculateParam))
                     .Returns(1);
            var function2 = CreateStrictFunctionMock("Function2");
            var function2RecalculateParam = new Candle() { };
            function2.SetupGet(f2 => f2.Value)
                     .Returns(2);
            function2.Setup(f2 => f2.AddNewValue(function2RecalculateParam))
                     .Returns(2);

            var functionService = CreateFunctionService(function1.Object, function2.Object);
            functionService.Initialize();

            // Warm-up
            functionService.Recalculate(CreateSingleCandlesResponse(
                new Dictionary<string, Candle>()
                {
                    {function1.Object.FunctionParameters.FunctionInstanceIdentifier, function1RecalculateParam },
                    {function2.Object.FunctionParameters.FunctionInstanceIdentifier, function2RecalculateParam },
                },
                function1.Object.FunctionParameters.FunctionInstanceIdentifier,
                function2.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue(function1.Object.FunctionParameters.FunctionInstanceIdentifier));
            Assert.AreEqual(2, results.GetValue(function2.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Verify the function warm-ups are invoked with the correct parameters
            function1.Verify();
            function2.Verify();
        }


        [Test]
        public void Recalculate_Trigered_OnSingleFunction()
        {
            var function1 = CreateStrictFunctionMock("Function1");
            var function1RecalculateParam = new Candle() { };
            function1.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function1.Setup(f1 => f1.AddNewValue(function1RecalculateParam))
                     .Returns(1);

            var functionService = CreateFunctionService(function1.Object);
            functionService.Initialize();

            // Warm-up
            functionService.Recalculate(CreateSingleCandlesResponse(
                new Dictionary<string, Candle>()
                {
                    {function1.Object.FunctionParameters.FunctionInstanceIdentifier, function1RecalculateParam },
                },
                function1.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue(function1.Object.FunctionParameters.FunctionInstanceIdentifier));

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
            var function = CreateStrictFunctionMock("Function1");
            var functionWarmUpParam = new List<Candle>() { new Candle() { } };
            function.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function.Setup(f1 => f1.WarmUp(functionWarmUpParam))
                     .Returns(1);

            var functionService = CreateFunctionService(function.Object);
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(CreateMultiCandlesResponse(
                new Dictionary<string, List<Candle>>()
                {
                    {function.Object.FunctionParameters.FunctionInstanceIdentifier, functionWarmUpParam }
                },
                function.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue(function.Object.FunctionParameters.FunctionInstanceIdentifier));

            functionService.Recalculate(new List<SingleCandleResponse>());

            // Assert function results
            var resultsAfterRecalculate = functionService.GetFunctionResults();
            Assert.AreEqual(1, resultsAfterRecalculate.GetValue(function.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Verify the function warm-ups are invoked with the correct parameters
            function.Verify();
        }

        [Test]
        public void Recalculate_Trigered_WhenNoCandles_AndNoWarmUp()
        {
            var function = CreateStrictFunctionMock("Function1");

            function.SetupGet(f => f.Value)
                    .Returns((double?)null);

            var functionService = CreateFunctionService(function.Object);
            functionService.Initialize();

            // Recalculate
            functionService.Recalculate(new List<SingleCandleResponse>());

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(null, results.GetValue(function.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Verify the function warm-ups are invoked with the correct parameters 
            function.Verify();
        }

        [Test]
        public void Recalculate_Trigered_WithNullValue()
        {
            var function1 = CreateStrictFunctionMock("Function1");
            var function1WarmUpParam = new List<Candle>() { new Candle() { } };
            function1.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function1.Setup(f1 => f1.WarmUp(function1WarmUpParam))
                     .Returns(1);
            var function2 = CreateStrictFunctionMock("Function2");
            var function2WarmUpParam = new List<Candle>() { };
            function2.SetupGet(f2 => f2.Value)
                     .Returns(2);
            function2.Setup(f2 => f2.WarmUp(function2WarmUpParam))
                     .Returns(2);

            var functionService = CreateFunctionService(function1.Object, function2.Object);
            functionService.Initialize();

            // Warm-up
            functionService.WarmUp(CreateMultiCandlesResponse(
                new Dictionary<string, List<Candle>>()
                {
                    {function1.Object.FunctionParameters.FunctionInstanceIdentifier, function1WarmUpParam },
                    {function2.Object.FunctionParameters.FunctionInstanceIdentifier, function2WarmUpParam },
                },
                function1.Object.FunctionParameters.FunctionInstanceIdentifier,
                function2.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue(function1.Object.FunctionParameters.FunctionInstanceIdentifier));
            Assert.AreEqual(2, results.GetValue(function2.Object.FunctionParameters.FunctionInstanceIdentifier));

            functionService.Recalculate(null);

            var resultsAfterRecalculate = functionService.GetFunctionResults();
            Assert.AreEqual(1, resultsAfterRecalculate.GetValue(function1.Object.FunctionParameters.FunctionInstanceIdentifier));
            Assert.AreEqual(2, resultsAfterRecalculate.GetValue(function2.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Verify the function warm-ups are invoked with the correct parameters 
            // (function1WarmUpParam and function2WarmUpParam)
            function1.Verify();
            function2.Verify();
        }

        [Test]
        public void Recalculate_Trigered_WithNullValue_AndNoWarmUp()
        {
            var function = CreateStrictFunctionMock("Function1");

            function.SetupGet(f1 => f1.Value)
                     .Returns((double?)null);

            var functionService = CreateFunctionService(function.Object);
            functionService.Initialize();

            // Warm-up
            functionService.Recalculate(null);

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(null, results.GetValue(function.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Verify the function warm-ups are invoked with the correct parameters 
            function.Verify();
        }

        [Test]
        public void Recalculate_WhenSomeFunctionDataIsMissing()
        {
            var function1 = CreateStrictFunctionMock("Function1");
            var function2 = CreateStrictFunctionMock("Function2");
            var function2RecalculateParam = new Candle() { };
            function1.SetupGet(f1 => f1.Value)
                     .Returns((double?)null);
            function2.Setup(f2 => f2.AddNewValue(function2RecalculateParam))
                     .Returns(2);
            function2.SetupGet(f2 => f2.Value)
                     .Returns(2);

            var functionService = CreateFunctionService(function1.Object, function2.Object);
            functionService.Initialize();

            // Warm-up
            functionService.Recalculate(CreateSingleCandlesResponse(
                new Dictionary<string, Candle>()
                {
                    // Do not provide function data for function1
                    {function2.Object.FunctionParameters.FunctionInstanceIdentifier, function2RecalculateParam }
                },
                function2.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(null, results.GetValue(function1.Object.FunctionParameters.FunctionInstanceIdentifier));
            Assert.AreEqual(2, results.GetValue(function2.Object.FunctionParameters.FunctionInstanceIdentifier));

            // Verify the function warm-ups are invoked with the correct parameters 
            // (function1WarmUpParam and function2WarmUpParam)
            function1.Verify();
            function2.Verify();
        }

        [Test]
        public void Recalculate_WhenExcessFunctionDataIsProvided()
        {
            var function1 = CreateStrictFunctionMock("Function1");
            var function1RecalculateParam = new Candle();
            function1.SetupGet(f1 => f1.Value)
                     .Returns(1);
            function1.Setup(f1 => f1.AddNewValue(function1RecalculateParam))
                     .Returns(1);
            var function2 = CreateStrictFunctionMock("Function2");
            var function2RecalculateParam = new Candle();
            function2.SetupGet(f2 => f2.Value)
                     .Returns(2);
            function2.Setup(f2 => f2.AddNewValue(function2RecalculateParam))
                     .Returns(2);

            var functionService = CreateFunctionService(function1.Object, function2.Object);
            functionService.Initialize();

            // Warm-up
            functionService.Recalculate(CreateSingleCandlesResponse(
                new Dictionary<string, Candle>()
                {
                    {"NON_EXISTING_FUNCTION",  new Candle()},
                    {function1.Object.FunctionParameters.FunctionInstanceIdentifier, function1RecalculateParam },
                    {"NON_EXISTING_FUNCTION_1", new Candle() },
                    {function2.Object.FunctionParameters.FunctionInstanceIdentifier, function2RecalculateParam },
                    {"NON_EXISTING_FUNCTION_2", new Candle()  },
                },
                "NON_EXISTING_FUNCTION",
                function1.Object.FunctionParameters.FunctionInstanceIdentifier,
                "NON_EXISTING_FUNCTION_1",
                function2.Object.FunctionParameters.FunctionInstanceIdentifier,
                "NON_EXISTING_FUNCTION_2"));

            // Assert function results
            var results = functionService.GetFunctionResults();
            Assert.AreEqual(1, results.GetValue(function1.Object.FunctionParameters.FunctionInstanceIdentifier));
            Assert.AreEqual(2, results.GetValue(function2.Object.FunctionParameters.FunctionInstanceIdentifier));

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

        #region GetFunctionMetadata

        [Test]
        public void GetCorrect_FunctionMetadata()
        {
            var algoSettingsService = GetIAlgoSettingsServiceMockReturnCorrectData();
            FunctionInitializationService service = new FunctionInitializationService(algoSettingsService);

            var functions = service.GetAllFunctions();

            var result = GetExampleFunctionResult();

            Then_Data_ShouldBe_Equal(functions, result);
        }

        [Test]
        public void GetCorrect_FunctionMetadata_Return_Empty_List()
        {
            var algoSettingsService = GetIAlgoSettingsServiceMock_ReturnNoAlgoInstance();
            FunctionInitializationService service = new FunctionInitializationService(algoSettingsService);

            var functions = service.GetAllFunctions();
            var result = new List<IFunction>();

            Then_Data_ShouldBe_Equal(functions, result);
        }

        private static void Then_Data_ShouldBe_Equal(IList<IFunction> first, IList<IFunction> second)
        {
            string serializedFirst = JsonConvert.SerializeObject(first);
            string serializedSecond = JsonConvert.SerializeObject(second);
            Assert.AreEqual(serializedFirst, serializedSecond);
        }


        private IList<IFunction> GetExampleFunctionResult()
        {
            var result = new List<IFunction>();

            result.Add(new SmaFunction(
                new SmaParameters()
                {
                    Capacity = 10,
                    FunctionInstanceIdentifier = "SAM_TEST",
                    AssetPair = "BTCEUR",
                    StartingDate = new DateTime(2018, 2, 10)
                }));

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

        private FunctionsService CreateFunctionService(params IFunction[] functions)
        {
            var functionInitializationService = new Mock<IFunctionInitializationService>();
            functionInitializationService.Setup(fis => fis.GetAllFunctions()).Returns(functions.ToList());
            return new FunctionsService(functionInitializationService.Object);
        }

        private Mock<IFunction> CreateStrictFunctionMock(string functionId)
        {
            var result = new Mock<IFunction>(MockBehavior.Strict);
            result.SetupGet(f => f.FunctionParameters)
                  .Returns(new FunctionParamsBase()
                  {
                      FunctionInstanceIdentifier = functionId
                  });
            return result;
        }

        private Mock<IFunction> CreateFunctionMockByParams(
            string functionId,
            string assetPair,
            CandleTimeInterval candleTimeInterval,
            DateTime startFrom)
        {
            var result = new Mock<IFunction>(MockBehavior.Strict);
            result.SetupGet(f => f.FunctionParameters)
                  .Returns(new FunctionParamsBase()
                  {
                      FunctionInstanceIdentifier = functionId,
                      AssetPair = assetPair,
                      CandleTimeInterval = candleTimeInterval,
                      StartingDate = startFrom
                  });
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
