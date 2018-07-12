using System;
using Lykke.AlgoStore.Algo.Indicators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.Algo
{
    public class BaseAlgo : IAlgo
    {
        // This will be set automatically during initialization
        private IIndicatorManager _paramProvider;

        // This method is used for code parser hinting.
        // Returning null is its correct functionality.
        protected T? Default<T>(T? value) where T : struct
        {
            return null;
        }

        protected T Default<T>(T value) where T : class
        {
            return null;
        }

        protected MACD MACD(
            string indicatorName,
            int? fastEmaPeriod = null, 
            int? slowEmaPeriod = null, 
            int? signalLinePeriod = null,
            DateTime? startingDate = null,
            DateTime? endingDate = null,
            string assetPair = null,
            CandleTimeInterval? candleTimeInterval = null,
            CandleOperationMode? candleOperationMode = null)
        {
            var indicator = new MACD(
                fastEmaPeriod ?? _paramProvider.GetParam<int>(indicatorName, nameof(fastEmaPeriod)),
                slowEmaPeriod ?? _paramProvider.GetParam<int>(indicatorName, nameof(slowEmaPeriod)),
                signalLinePeriod ?? _paramProvider.GetParam<int>(indicatorName, nameof(signalLinePeriod)),
                startingDate ?? _paramProvider.GetParam<DateTime>(indicatorName, nameof(startingDate)),
                endingDate ?? _paramProvider.GetParam<DateTime>(indicatorName, nameof(endingDate)),
                candleTimeInterval ?? _paramProvider.GetParam<CandleTimeInterval>(indicatorName, nameof(candleTimeInterval)),
                assetPair ?? _paramProvider.GetParam<string>(indicatorName, nameof(assetPair)),
                candleOperationMode ?? _paramProvider.GetParam<CandleOperationMode>(indicatorName, nameof(candleOperationMode))
            );
            _paramProvider.RegisterIndicator(indicatorName, indicator);

            return indicator;
        }

        protected EMA EMA(
            string indicatorName,
            int? period = null,
            DateTime? startingDate = null,
            DateTime? endingDate = null,
            string assetPair = null,
            CandleTimeInterval? candleTimeInterval = null,
            CandleOperationMode? candleOperationMode = null)
        {
            var indicator = new EMA(
                period ?? _paramProvider.GetParam<int>(indicatorName, nameof(period)),
                startingDate ?? _paramProvider.GetParam<DateTime>(indicatorName, nameof(startingDate)),
                endingDate ?? _paramProvider.GetParam<DateTime>(indicatorName, nameof(endingDate)),
                candleTimeInterval ?? _paramProvider.GetParam<CandleTimeInterval>(indicatorName, nameof(candleTimeInterval)),
                assetPair ?? _paramProvider.GetParam<string>(indicatorName, nameof(assetPair)),
                candleOperationMode ?? _paramProvider.GetParam<CandleOperationMode>(indicatorName, nameof(candleOperationMode))
            );

            _paramProvider.RegisterIndicator(indicatorName, indicator);
            return indicator;
        }

        protected SMA SMA(
            string indicatorName,
            int? period = null,
            DateTime? startingDate = null,
            DateTime? endingDate = null,
            string assetPair = null,
            CandleTimeInterval? candleTimeInterval = null,
            CandleOperationMode? candleOperationMode = null)
        {
            var indicator = new SMA(
                period ?? _paramProvider.GetParam<int>(indicatorName, nameof(period)),
                startingDate ?? _paramProvider.GetParam<DateTime>(indicatorName, nameof(startingDate)),
                endingDate ?? _paramProvider.GetParam<DateTime>(indicatorName, nameof(endingDate)),
                candleTimeInterval ?? _paramProvider.GetParam<CandleTimeInterval>(indicatorName, nameof(candleTimeInterval)),
                assetPair ?? _paramProvider.GetParam<string>(indicatorName, nameof(assetPair)),
                candleOperationMode ?? _paramProvider.GetParam<CandleOperationMode>(indicatorName, nameof(candleOperationMode))
            );

            _paramProvider.RegisterIndicator(indicatorName, indicator);
            return indicator;
        }

        protected ADX ADX(
            string indicatorName,
            int? period = null,
            DateTime? startingDate = null,
            DateTime? endingDate = null,
            string assetPair = null,
            CandleTimeInterval? candleTimeInterval = null)
        {
            var indicator = new ADX(
                period ?? _paramProvider.GetParam<int>(indicatorName, nameof(period)),
                startingDate ?? _paramProvider.GetParam<DateTime>(indicatorName, nameof(startingDate)),
                endingDate ?? _paramProvider.GetParam<DateTime>(indicatorName, nameof(endingDate)),
                candleTimeInterval ?? _paramProvider.GetParam<CandleTimeInterval>(indicatorName, nameof(candleTimeInterval)),
                assetPair ?? _paramProvider.GetParam<string>(indicatorName, nameof(assetPair))
            );

            _paramProvider.RegisterIndicator(indicatorName, indicator);
            return indicator;
        }

        protected ATR ATR(
            string indicatorName,
            int? period = null,
            DateTime? startingDate = null,
            DateTime? endingDate = null,
            string assetPair = null,
            CandleTimeInterval? candleTimeInterval = null)
        {
            var indicator = new ATR(
                period ?? _paramProvider.GetParam<int>(indicatorName, nameof(period)),
                startingDate ?? _paramProvider.GetParam<DateTime>(indicatorName, nameof(startingDate)),
                endingDate ?? _paramProvider.GetParam<DateTime>(indicatorName, nameof(endingDate)),
                candleTimeInterval ?? _paramProvider.GetParam<CandleTimeInterval>(indicatorName, nameof(candleTimeInterval)),
                assetPair ?? _paramProvider.GetParam<string>(indicatorName, nameof(assetPair))
            );

            _paramProvider.RegisterIndicator(indicatorName, indicator);
            return indicator;
        }

        protected DMI DMI(
            string indicatorName,
            int? period = null,
            DateTime? startingDate = null,
            DateTime? endingDate = null,
            string assetPair = null,
            CandleTimeInterval? candleTimeInterval = null)
        {
            var indicator = new DMI(
                period ?? _paramProvider.GetParam<int>(indicatorName, nameof(period)),
                startingDate ?? _paramProvider.GetParam<DateTime>(indicatorName, nameof(startingDate)),
                endingDate ?? _paramProvider.GetParam<DateTime>(indicatorName, nameof(endingDate)),
                candleTimeInterval ?? _paramProvider.GetParam<CandleTimeInterval>(indicatorName, nameof(candleTimeInterval)),
                assetPair ?? _paramProvider.GetParam<string>(indicatorName, nameof(assetPair))
            );

            _paramProvider.RegisterIndicator(indicatorName, indicator);
            return indicator;
        }

        public virtual void OnCandleReceived(ICandleContext context)
        {
        }

        public virtual void OnQuoteReceived(IQuoteContext context)
        {
        }

        public virtual void OnStartUp(IFunctionProvider functions)
        {
        }

        public string AssetPair { get; set; }
        public CandleTimeInterval CandleInterval { get; set; }
        public DateTime StartFrom { get; set; }
        public DateTime EndOn { get; set; }
        public double Volume { get; set; }
        public string TradedAsset { get; set; }
    }
}
