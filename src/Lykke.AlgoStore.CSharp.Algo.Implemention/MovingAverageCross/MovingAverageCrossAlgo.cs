using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.ADX;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.SMA;

namespace Lykke.AlgoStore.CSharp.Algo.Implemention.MovingAverageCross
{
    /// <summary>
    /// Moving Average Cross Algorithm
    /// </summary>
    public class MovingAverageCrossAlgo : BaseAlgo
    {
        //public MovingAverageCrossParameters Parameters { get; set; }
        private bool _cross { get; set; }

        private int _adxThreshold = 10;

        private double _lastSMAShort;
        private double _lastSMALong;
        private double? _lastADXValue { get; set; }

        private SmaFunction _smaShortPeriod;
        private SmaFunction _smaLongPeriod;
        private AdxFunction _adx;

        public override void OnStartUp(IFunctionProvider functions)
        {
            //TODO  we should get params
            //_adxThreshold = Convert.ToInt32(context.Parameters.GetParameterValue("AdxThreshold"));

            _smaShortPeriod = functions.GetFunction<SmaFunction>("SMA_Short");
            _smaLongPeriod = functions.GetFunction<SmaFunction>("SMA_Long");
            _adx = functions.GetFunction<AdxFunction>("ADX");

            _lastSMAShort = _smaShortPeriod.Value ?? 0;
            _lastSMALong = _smaLongPeriod.Value ?? 0;
            _lastADXValue = _adx.Value;
        }

        public override void OnCandleReceived(ICandleContext contextCandle)
        {
            var currentSMAShort = _smaShortPeriod.Value ?? 0;
            var currentSMALong = _smaLongPeriod.Value ?? 0;
            double? adx = _adx.Value;

            contextCandle.Actions.Log($"SMA_Short: {currentSMAShort}, SMA_Long: {currentSMALong}, ADX: {adx}");

            _cross = false;

            if (adx.HasValue && adx > _adxThreshold)
            {
                if (_lastSMAShort < _lastSMALong && currentSMAShort > currentSMALong)
                {
                    //buy
                    contextCandle.Actions.Log($"Cross occurred Buy => SMA_Short: {currentSMAShort}, SMA_Long: {currentSMALong}");
                    _cross = true;
                }

                if (_lastSMAShort > _lastSMALong && currentSMAShort < currentSMALong)
                {
                    //Sell
                    contextCandle.Actions.Log($"Cross occurred Sell => SMA_Short: {currentSMAShort}, SMA_Long: {currentSMALong}");
                    _cross = true;
                }
            }


            _lastSMALong = currentSMALong;
            _lastSMAShort = currentSMAShort;
            _lastADXValue = adx;
        }

        public double GetSMAShortTerm() => _lastSMAShort;

        public double GetSMALongTerm() => _lastSMALong;

        public double? GetADX() => _lastADXValue;

        public bool GetCross() => _cross;
    }
}
