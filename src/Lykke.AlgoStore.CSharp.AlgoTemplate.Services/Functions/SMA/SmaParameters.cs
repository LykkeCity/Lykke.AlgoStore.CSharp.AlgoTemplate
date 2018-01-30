namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.SMA
{
    /// <summary>
    /// Simple Moving Average (SMA) parameters
    /// </summary>
    public class SmaParameters
    {
        /// <summary>
        /// Short term period (window)
        /// </summary>
        public int ShortTermPeriod { get; set; }

        /// <summary>
        /// Long term period (window)
        /// </summary>
        public int LongTermPeriod { get; set; }

        /// <summary>
        /// Number of decimals
        /// </summary>
        public int? Decimals { get; set; }
    }
}
