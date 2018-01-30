using System.Globalization;
using System.Text;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Functions.SMA
{
    public class SmaFunction
    {
        public SmaParameters Parameters { get; set; }

        public double[] Values { get; set; }

        public double[] SmaLongTerm { get; private set; }

        public double[] SmaShortTerm { get; private set; }

        public void CalculateSma()
        {
            SmaLongTerm = Utils.CalculateSma(Values, Parameters.LongTermPeriod, Parameters.Decimals);
            SmaShortTerm = Utils.CalculateSma(Values, Parameters.ShortTermPeriod, Parameters.Decimals);
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
