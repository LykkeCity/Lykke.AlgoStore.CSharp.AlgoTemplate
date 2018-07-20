﻿using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Algo.Indicators;

namespace Lykke.AlgoStore.CSharp.Algo.Implemention
{
    /// <summary>
    /// Moving Average Cross Algorithm
    /// </summary>
    public class MovingAverageCrossAlgo : BaseAlgo
    {
        public int ADXThreshold { get; set; }

        private bool _crossSMAShortAbove;
        private bool _crossSMAShortBelow;

        private double _lastSMAShort;
        private double _lastSMALong;
        private double? _lastADXValue;

        private double _currentSMAShort;
        private double _currentSMALong;
        private double? _currentADX;

        private SMA _smaShortPeriod;
        private SMA _smaLongPeriod;
        private ADX _adx;

        public override void OnStartUp()
        {
            _smaShortPeriod = SMA("SMA_Short");
            _smaLongPeriod = SMA("SMA_Long");
            _adx = ADX("ADX");

            _currentSMAShort = _smaShortPeriod.Value ?? 0;
            _currentSMALong = _smaLongPeriod.Value ?? 0;
            _currentADX = _adx.Value;

            _lastSMALong = _currentSMALong;
            _lastSMAShort = _currentSMAShort;
            _lastADXValue = _currentADX;
        }

        public override void OnCandleReceived(ICandleContext contextCandle)
        {
            contextCandle.Actions.Log($"Algo ADX Threshold {ADXThreshold}");
            contextCandle.Actions.Log($"SMA_Short Asset Pair: {_smaShortPeriod.AssetPair}, " +
                                      $"SMA_Long Asset Pair: {_smaLongPeriod.AssetPair}");

            _currentSMAShort = _smaShortPeriod.Value ?? 0;
            _currentSMALong = _smaLongPeriod.Value ?? 0;
            _currentADX = _adx.Value;

            _crossSMAShortAbove = false;
            _crossSMAShortBelow = false;

            contextCandle.Actions.Log($"Function values are: SMA_Short: {_currentSMAShort}, SMA_Long: {_currentSMALong}, ADX: {_currentADX}");


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

            if (_currentADX.HasValue && _currentADX > ADXThreshold)
            {
                if (_crossSMAShortAbove)
                {
                    contextCandle.Actions.Log($"Cross above and ADX occurred BUY => SMA_Short: {_currentSMAShort}, SMA_Long: {_currentSMALong}");
                    //context.Actions.BuyStraight(parameter.ValueToBuy);
                }

                if (_crossSMAShortBelow)
                    contextCandle.Actions.Log($"Cross below and ADX occurred SELL => SMA_Short: {_currentSMAShort}, SMA_Long: {_currentSMALong}");
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
