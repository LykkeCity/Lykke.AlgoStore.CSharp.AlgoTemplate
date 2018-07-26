using System;
using Lykke.AlgoStore.Algo.Indicators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.Algo
{
    public class BaseAlgo : IAlgo
    {
        // This will be set automatically during initialization
        private IIndicatorManager _paramProvider;

        #region Defaults

        // These methods are used for code parser hinting.
        // Returning null is their correct functionality.
        protected T? Default<T>(T? value) where T : struct => null;
        protected T Default<T>(T value) where T : class => null;
        protected bool? Default(bool value) => null;
        protected byte? Default(byte value) => null;
        protected sbyte? Default(sbyte value) => null;
        protected int? Default(int value) => null;
        protected uint? Default(uint value) => null;
        protected short? Default(short value) => null;
        protected ushort? Default(ushort value) => null;
        protected long? Default(long value) => null;
        protected ulong? Default(ulong value) => null;

        #endregion // Defaults

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

        public virtual void OnStartUp()
        {
        }

        public string AssetPair { get; }
        public CandleTimeInterval CandleInterval { get; }
        public DateTime StartFrom { get; }
        public DateTime EndOn { get; }
        public double Volume { get; }
        public string TradedAsset { get; }
    }
}
