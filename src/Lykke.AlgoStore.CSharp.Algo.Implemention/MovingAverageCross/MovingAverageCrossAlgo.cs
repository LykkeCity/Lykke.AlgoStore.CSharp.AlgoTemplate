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
        private bool _crossSMAShortAbove { get; set; }
        private bool _crossSMAShortBelow { get; set; }

        private int _adxThreshold = 10;

        private double _lastSMAShort;
        private double _lastSMALong;
        private double? _lastADXValue;

        private double _currentSMAShort;
        private double _currentSMALong;
        private double? _currentADX;

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

            _currentSMAShort = _smaShortPeriod.Value ?? 0;
            _currentSMALong = _smaLongPeriod.Value ?? 0;
            _currentADX = _adx.Value;

            _lastSMALong = _currentSMALong;
            _lastSMAShort = _currentSMAShort;
            _lastADXValue = _currentADX;
        }

        public override void OnCandleReceived(ICandleContext contextCandle)
        {
            _currentSMAShort = _smaShortPeriod.Value ?? 0;
            _currentSMALong = _smaLongPeriod.Value ?? 0;
            _currentADX = _adx.Value;

            _crossSMAShortAbove = false;
            _crossSMAShortBelow = false;

            //we want to mark the cross and later check if there is adx
            if (_lastSMAShort < _lastSMALong && _currentSMAShort > _currentSMALong)
            {
                contextCandle.Actions.Log($"Cross above of the SMA short period function => SMA_Short: {_currentSMAShort}, SMA_Long: {_currentSMALong}");
                _crossSMAShortAbove = true;
            }

            if (_lastSMAShort > _lastSMALong && _currentSMAShort < _currentSMALong)
            {
                contextCandle.Actions.Log($"Cross below of the SMA short period function => SMA_Short: {_currentSMAShort}, SMA_Long: {_currentSMALong}");
                _crossSMAShortBelow = true;
            }

            //if we have adx and cross we can buy/sell
            //TODO we should set in parameter value for sell and buy if 
            //it is one and the same we can use delegates for calling trading methods

            if (_currentADX.HasValue && _currentADX > _adxThreshold)
            {
                if (_crossSMAShortAbove)
                {
                    contextCandle.Actions.Log($"Cross above and ADX occurred BUY => SMA_Short: {_currentSMAShort}," +
                        $"                           SMA_Long: {_currentSMALong}");
                    //context.Actions.BuyStraight(parameter.ValueToBuy);
                }

                if (_crossSMAShortBelow)
                    contextCandle.Actions.Log($"Cross below and ADX occurred SELL => SMA_Short: {_currentSMAShort}, " +
                        $"                          SMA_Long: {_currentSMALong}");
                //context.Actions.SellStraight(parameter.ValueToSell);
            }

            _lastSMALong = _currentSMALong;
            _lastSMAShort = _currentSMAShort;
            _lastADXValue = _currentADX;
        }

        public double GetSMAShortTerm() => _currentSMAShort;

        public double GetSMALongTerm() => _currentSMALong;

        public double? GetADX() => _currentADX;

        public bool GetCrossSMAShortBelow() => _crossSMAShortBelow;

        public bool GetCrossSMAShortAbove() => _crossSMAShortAbove;
    }
}
