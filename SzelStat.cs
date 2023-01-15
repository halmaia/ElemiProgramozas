using Microsoft.VisualBasic.FileIO;
using System.Runtime.CompilerServices;

namespace CSV
{
    internal static class Program
    {
        internal static void Main()
        {
            KahanBabuska sinsum = new(), cossum = new(); int hit = 0;
            using TextFieldParser parser = new(@"F:\szel.csv")
            {
                TrimWhiteSpace = true,
                Delimiters = new string[] { "," },
                HasFieldsEnclosedInQuotes = false,
                TextFieldType = FieldType.Delimited
            };
            parser.ReadLine(); // Header

            while (!parser.EndOfData)
            {
                var line = parser.ReadFields();
                if (line?.Length == 7 &&
                    float.TryParse(line[5], out float ws) &&
                    ws > 0f &&
                    ushort.TryParse(line[6], out ushort wd))
                {
                    (double Sin, double Cos) sincos = double.SinCos((Math.Tau / 360d) * wd);
                    sinsum.Add(sincos.Item1); cossum.Add(sincos.Item2);
                    hit++;
                }
            }
            parser.Close();
            
            Console.WriteLine((360 / double.Tau) * double.Atan2(sinsum.GetResult() / hit, cossum.GetResult() / hit));
        }
    }
    internal class KahanBabuska
    {
        private double sum, com;

        internal KahanBabuska(double startValue = 0) =>
            sum = startValue;
        [SkipLocalsInit]
        internal void Add(double value)
        {
            double sum = this.sum,
                   n = sum + value;
            com += Math.Abs(sum) >= Math.Abs(value) ?
                (sum - n) + value :
                (value - n) + sum;
            this.sum = n;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Reset()
        {
            com = 0; sum = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal double GetResult() =>
            sum + com;
    }
}
