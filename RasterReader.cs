using System.Buffers.Text;

namespace TestAAIGridReader
{
    internal class Program
    {
        private static void Main()
        {
            Span<double> raster = stackalloc double[15];
            GetRasterArray(@"F:\f.txt", ref raster);
        }



        private static void GetRasterArray(string path, ref Span<double> raster)
        {
            using FileStream fileStream = File.OpenRead(path);
            long streamLength = fileStream.Length;
            Span<byte> buffer = stackalloc byte[(int)Math.Min(4096L, streamLength)];
            int r = 0;

            int bufferLength;        
            while ((bufferLength = fileStream.Read(buffer)) != 0)
                for (int i = 0; i < bufferLength; i++)
                {

                    byte chr;
                    while (((chr = buffer[i]) == 32 || (chr > 8 && chr < 14)) && ++i < bufferLength) { }

                    if (!Utf8Parser.TryParse(buffer[i..bufferLength], out double value, out int bytesConsumed))
                        continue;

                    if ((i += bytesConsumed) == bufferLength)
                    {
                        if (fileStream.Position == streamLength)
                        {
                            raster[r] = value;
                            return;
                        }

                        fileStream.Seek(-bytesConsumed, SeekOrigin.Current);
                        break;
                    }
                    raster[r++] = value;
                }

            return;
        }
    }
}
