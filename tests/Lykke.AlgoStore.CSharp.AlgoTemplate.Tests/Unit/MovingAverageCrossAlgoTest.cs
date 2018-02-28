using Lykke.AlgoStore.CSharp.Algo.Core.Candles;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.Algo.Implemention;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.MovingAverageCross;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Unit
{
    [TestFixture]
    public class MovingAverageCrossAlgoTest
    {
        private readonly string _fileNameCorrectData = "Adx_Examples2.txt";
        public readonly string _fileNameNotFullData = "Adx_Examples.txt";
        private const int DEFAULT_PERCISION = 14;

        public IList<Candle> GetTestCandles(string externalDataFilename)
        {

            List<Candle> candles = new List<Candle>();

            bool first = true;
            int targetIndex = -1;
            bool fileHasVolume = false;
            foreach (var line in File.ReadLines(Path.Combine("TestData", externalDataFilename)))
            {
                var parts = line.Split(',');
                if (first)
                {
                    fileHasVolume = parts[2].Trim() == "Volume";
                    first = false;
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (parts[i].Trim() == "ADX 14")
                        {
                            targetIndex = i;
                            break;
                        }
                    }
                    continue;
                }

                candles.Add(new Candle
                {
                    DateTime = DateTime.Now,
                    Open = 0,
                    High = Convert.ToDouble(parts[0]),
                    Low = Convert.ToDouble(parts[1]),
                    Close = Convert.ToDouble(parts[2]),
                });
            }
            return candles;
        }

        public IContext MockContext(Candle candle)
        {
            var contextMock = new Mock<IContext>();
            //contextMock.Setup(c => c.CandleData.GetGetCandle()).Returns
            //(
            //    new AlgoCandle
            //    {
            //        ClosePrice = candle.Close,
            //        HighPrice = candle.High,
            //        LowPrice = candle.Low,
            //        Timestamp = candle.DateTime
            //    }
            //);

            contextMock.Setup(c => c.Functions.GetValue("SMA_Short")).Returns
                (
                20
                );

            contextMock.Setup(c => c.Functions.GetValue("SMA_Long")).Returns
               (
               30
               );

            contextMock.Setup(c => c.Functions.GetValue("ADX")).Returns
               (
               2
               );
            return contextMock.Object;
        }

        [Test]
        public void MovingAverageTest()
        {
            var function = new MovingAverageCrossAlgo
            {
                //Parameters = new MovingAverageCrossParameters
                //{
                //    LongTermPeriod = 14,
                //    ShortTermPeriod = 10
                //}
            };
            var values = GetTestCandles("Adx_Examples2.txt");

            double? adxValue = 0.0d;
            double valueToCheck = 20.39;

            //function.OnCandleReceived(MockContext(values[0]));

            //function.WarmUp(values);

        }
    }
}
