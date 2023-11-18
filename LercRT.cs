using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LercCS
{
    internal static class Program
    {
        private static unsafe ReadOnlySpan<byte> Encode(ReadOnlySpan<byte> data,
            uint dataType, int nDepth, int nCols, int nRows, int nBands,
            int nMasks, ReadOnlySpan<byte> validBytes,
            double maxZErr)
        {

            fixed (byte* ptr = data)
            {
                ReadOnlySpan<byte> buffer = GC.AllocateUninitializedArray<byte>(8192);
                fixed (byte* pOutBuffer = buffer)
                {
                    Unsafe.SkipInit<uint>(out uint nBytesWritten);
                    int success = Lerc_Encode(ptr, dataType, nDepth, nCols, nRows, nBands, nMasks, null,
                        maxZErr, pOutBuffer, 8192, &nBytesWritten);
                    return buffer[..(int)nBytesWritten];
                }
            }

            [DllImport(@"Lerc.dll", EntryPoint = @"lerc_encode")]
            static extern unsafe int Lerc_Encode(
                void* pData,                 // raw image data, row by row, band by band
      uint dataType,             // char = 0, uchar = 1, short = 2, ushort = 3, int = 4, uint = 5, float = 6, double = 7
      int nDepth,                        // number of values per pixel (e.g., 3 for RGB, data is stored as [RGB, RGB, ...])
      int nCols,                         // number of columns
      int nRows,                         // number of rows
      int nBands,                        // number of bands (e.g., 3 for [RRRR ..., GGGG ..., BBBB ...])
      int nMasks,                        // 0 - all valid, 1 - same mask for all bands, nBands - masks can differ between bands
      byte* pValidBytes,  // nullptr if all pixels are valid; otherwise 1 byte per pixel (1 = valid, 0 = invalid)
      double maxZErr,                    // max coding error per pixel, defines the precision
      byte* pOutBuffer,         // buffer to write to, function fails if buffer too small
      uint outBufferSize,        // size of output buffer
      uint* nBytesWritten);      // number of bytes written to output buffer
        }
        private static unsafe void Main()
        {
            var rnd = Random.Shared;
            float[] ras = new float[64];
            for (int i = 0; i < 64;)
            {
                ras[i++] = float.FusedMultiplyAdd(40f, rnd.NextSingle(), 160f);
            }

            var comp_ras = Encode(MemoryMarshal.AsBytes<float>(ras), 6, 1, 8, 8, 1, 0, null, .1d);

            fixed (byte* rasPointer = comp_ras)
            {
                if (new string((sbyte*)rasPointer, 0, 6) != @"Lerc2 ")
                    throw new InvalidDataException("Invalid file!");
                HeaderInfo* info = (HeaderInfo*)(rasPointer + 6);
                FloatRange* range = (FloatRange*)(((byte*)info) + sizeof(HeaderInfo));
                byte* ptr = ((byte*)range) + sizeof(FloatRange);
                bool isRaw = *ptr is 1;
                BlockAndArrayHeader* header = (BlockAndArrayHeader*)(ptr+=1);


                var decoded = GC.AllocateUninitializedArray<float>(header->numPix);
                var offset = header->offset;
                float dmaxZ = (float)(2d * info->MaxZError);
                ptr+= sizeof(BlockAndArrayHeader);
                for (int i = 0, max = header->numPix;
                    i < max; i++)
                {
                   decoded[i]= float.FusedMultiplyAdd(dmaxZ, ptr[i], offset);
                }


            }


        }
        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 88)]
        private readonly ref struct HeaderInfo
        {
            public readonly int Version;
            public readonly uint Checksum;
            public readonly int Height;
            public readonly int Width;
            public readonly int ValuesPerPixel;
            public readonly int NumberOfValidPixels;
            public readonly int MicroBlockSize;
            public readonly int BlobSize;
            public readonly int DataType;

            public readonly int NumberOfMoreBlobs;         // for multi-band Lerc blob, how many more blobs or bands appended to this one

            public readonly byte UsesNoData;  // 1 - pass noData values to decoder, 0 - don't pass, ignore
            public readonly byte DoublesAreInts;    // 1 - float or double data is all integer numbers, 0 - not
            public readonly byte Reserved1;
            public readonly byte Reserved2;

            public readonly double MaxZError;
            public readonly double zMin;    // if nDepth > 1, this is the overall range
            public readonly double zMax;
            public readonly double noDataVal;      // temp noData value used for nDepth > 1 if bit mask cannot cover it
            public readonly double noDataValOrig;  // orig noData value to map to in decode
            public readonly int maskBlobSize;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2 * sizeof(float))]
        private readonly ref struct FloatRange

        {
            internal readonly float zMin;
            internal readonly float zMax;
        }

        private enum EncodingType:byte
        {
            Raw, Quantized, AllZero, AllConstant
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size =7)]
        private readonly ref struct BlockAndArrayHeader

        {
            internal readonly EncodingType type;
            internal readonly float offset;
            internal readonly byte BitsAndType;
          
            internal readonly byte numPix;

            internal readonly byte numBits => (byte)(0b0001_1111 & BitsAndType);
            internal readonly byte numFxLen => (byte)( BitsAndType>>6);

        }
    }
}
