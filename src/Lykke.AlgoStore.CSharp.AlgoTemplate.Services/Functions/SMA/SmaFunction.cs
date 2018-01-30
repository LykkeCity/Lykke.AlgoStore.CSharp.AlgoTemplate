using System.Globalization;
using System.Text;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.SMA
{
    /// <summary>
    /// Simple Moving Average (SMA) function
    /// </summary>
    public class SmaFunction
    {
        /// <summary>
        /// Parameters <see cref="SmaParameters"/>
        /// </summary>
        public SmaParameters Parameters { get; set; }

        /// <summary>
        /// Values that are used to calculate SMA
        /// </summary>
        public double[] Values { get; set; }

        /// <summary>
        /// SMA calculated values based on long period (window)
        /// </summary>
        public double[] SmaLongTerm { get; private set; }

        /// <summary>
        /// SMA calculated values based on short period (window)
        /// </summary>
        public double[] SmaShortTerm { get; private set; }

        /// <summary>
        /// Calculate SMA
        /// </summary>
        public void CalculateSma()
        {
            SmaLongTerm = Utils.SmaUtils.CalculateSma(Values, Parameters.LongTermPeriod, Parameters.Decimals);
            SmaShortTerm = Utils.SmaUtils.CalculateSma(Values, Parameters.ShortTermPeriod, Parameters.Decimals);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{"Price".PadLeft(20)}{"SmaShort".PadLeft(20)}{"SmaLong".PadLeft(20)}");
            sb.AppendLine("-".PadLeft(60, '-'));

            for (int i = 0; i < Values.Length; i++)
            {
                sb.AppendLine(
                    $"{Values[i].ToString(CultureInfo.InvariantCulture).PadLeft(20)}{SmaShortTerm[i].ToString(CultureInfo.InvariantCulture).PadLeft(20)}{SmaLongTerm[i].ToString(CultureInfo.InvariantCulture).PadLeft(20)}");
            }

            sb.AppendLine("-".PadLeft(60, '-'));

            return sb.ToString();
        }
    }
}
