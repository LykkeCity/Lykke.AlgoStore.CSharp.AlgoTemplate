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
            [Description("The amount of most recent values that the Fast EMA will operate on.")]
            int? fastEmaPeriod = null,
            [Description("The amount of most recent values that the Slow EMA will operate on.")]
            int? slowEmaPeriod = null,
            [Description("The amount of most recent values that the Single Line will operate on.")]
            int? signalLinePeriod = null,
            [Description("The starting date of the indicator.")]
            DateTime? startingDate = null,
            [Description("The ending date of the indicator.")]
            DateTime? endingDate = null,
            [Description("The asset pair that the indicator will be using.")]
            string assetPair = null,
            [Description("The interval that the candles will be received on.")]
            CandleTimeInterval? candleTimeInterval = null,
            [Description("The candle value on which the function is operating. The same function can be operating on Min/Max or Open/Close of a Candle.")]
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
            [Description("The amount of most recent values this indicator will operate on.")]
            int? period = null,
            [Description("The starting date of the indicator.")]
            DateTime? startingDate = null,
            [Description("The ending date of the indicator.")]
            DateTime? endingDate = null,
            [Description("The asset pair that the indicator will be using.")]
            string assetPair = null,
            [Description("The interval that the candles will be received on.")]
            CandleTimeInterval? candleTimeInterval = null,
            [Description("The candle value on which the function is operating. The same function can be operating on Min/Max or Open/Close of a Candle.")]
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
            [Description("The amount of most recent values this indicator will operate on.")]
            int? period = null,
            [Description("The starting date of the indicator.")]
            DateTime? startingDate = null,
            [Description("The ending date of the indicator.")]
            DateTime? endingDate = null,
            [Description("The asset pair that the indicator will be using.")]
            string assetPair = null,
            [Description("The interval that the candles will be received on.")]
            CandleTimeInterval? candleTimeInterval = null,
            [Description("The candle value on which the function is operating. The same function can be operating on Min/Max or Open/Close of a Candle.")]
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
            [Description("The amount of most recent values this indicator will operate on.")]
            int? period = null,
            [Description("The starting date of the indicator.")]
            DateTime? startingDate = null,
            [Description("The ending date of the indicator.")]
            DateTime? endingDate = null,
            [Description("The asset pair that the indicator will be using.")]
            string assetPair = null,
            [Description("The interval that the candles will be received on.")]
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
            [Description("The amount of most recent values this indicator will operate on.")]
            int? period = null,
            [Description("The starting date of the indicator.")]
            DateTime? startingDate = null,
            [Description("The ending date of the indicator.")]
            DateTime? endingDate = null,
            [Description("The asset pair that the indicator will be using.")]
            string assetPair = null,
            [Description("The interval that the candles will be received on.")]
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
            [Description("The amount of most recent values this indicator will operate on.")]
            int? period = null,
            [Description("The starting date of the indicator.")]
            DateTime? startingDate = null,
            [Description("The ending date of the indicator.")]
            DateTime? endingDate = null,
            [Description("The asset pair that the indicator will be using.")]
            string assetPair = null,
            [Description("The interval that the candles will be received on.")]
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

        [Description("The asset pair that the algorithm will be using.")]
        public string AssetPair { get; }
        [Description("The interval that the candles will be received.")]
        public CandleTimeInterval CandleInterval { get; }
        [Description("The starting date of the algorithm.")]
        public DateTime StartFrom { get; }
        [Description("The ending date of the algorithm.")]
        public DateTime EndOn { get; }
        [Description("The volume that your algorithm will trade.")]
        public double Volume { get; }
        [Description("The asset that your algorithm will use for trading.")]
        public string TradedAsset { get; }
    }
}
