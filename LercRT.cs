using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

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
            float[] ras = new float[256];
            for (int i = 0; i < 256;)
            {
                // 40; 160
                ras[i++] = float.FusedMultiplyAdd(10f, rnd.NextSingle(), 160f);
            }

            ReadOnlySpan<byte> comp_ras = Encode(MemoryMarshal.AsBytes<float>(ras), 6, 1, 16, 16, 1, 0, null, .1d);

            if (!comp_ras.StartsWith(@"Lerc2 "u8))
                throw new InvalidDataException("Invalid file!");

            fixed (byte* rasPointer = comp_ras[6..])
            {
                HeaderInfo* info = (HeaderInfo*)(rasPointer);
                FloatRange* range = (FloatRange*)(((byte*)info) + sizeof(HeaderInfo));
                byte* ptr = ((byte*)range) + sizeof(FloatRange);
                bool isRaw = *ptr is 1;
                BlockAndArrayHeader* header = (BlockAndArrayHeader*)(ptr += 1);


                var decoded = GC.AllocateUninitializedArray<float>(header->numPix);
                var offset = header->offset;
                float dmaxZ = (float)(2d * info->MaxZError);
                ptr += sizeof(BlockAndArrayHeader);

                var mux = Decompress8bit8x8BlocksToFloat(new(ptr, 64), (float)info->MaxZError, header->offset);


                for (int i = 0, max = header->numPix;
                    i < max; i++)
                {
                    decoded[i] = float.FusedMultiplyAdd(dmaxZ, ptr[i], offset);
                }


                ptr += 64;
                header = (BlockAndArrayHeader*)(ptr);



                static unsafe ReadOnlySpan<float> Decompress8bit8x8BlocksToFloat(
                       ReadOnlySpan<byte> bytesToDecompress,
                       in float maxZError, float offset)
                {
                    fixed (byte* ptr = bytesToDecompress)
                    {
                        ReadOnlySpan<float> result = new float[64];
                        fixed (float* dst = result)
                        {
                            var z = Vector256.Create(2.0f * maxZError);
                            var min = Avx2.BroadcastScalarToVector256(&offset);

                            (var ushort1, var ushort2) = Vector256.Widen(*(Vector256<byte>*)ptr);
                            (var uint1, var uint2) = Vector256.Widen(ushort1);
                            Fma.MultiplyAdd(z, Vector256.ConvertToSingle(uint1), min).Store(dst);
                            Fma.MultiplyAdd(z, Vector256.ConvertToSingle(uint2), min).Store(dst + 8);

                            (var uint3, var uint4) = Vector256.Widen(ushort2);
                            Fma.MultiplyAdd(z, Vector256.ConvertToSingle(uint3), min).Store(dst + 16);
                            Fma.MultiplyAdd(z, Vector256.ConvertToSingle(uint4), min).Store(dst + 24);

                            (var ushort3, var ushort4) = Vector256.Widen(*(Vector256<byte>*)(ptr + 32));
                            (var uint5, var uint6) = Vector256.Widen(ushort3);
                            Fma.MultiplyAdd(z, Vector256.ConvertToSingle(uint5), min).Store(dst + 32);
                            Fma.MultiplyAdd(z, Vector256.ConvertToSingle(uint6), min).Store(dst + 40);

                            (var uint7, var uint8) = Vector256.Widen(ushort4);
                            Fma.MultiplyAdd(z, Vector256.ConvertToSingle(uint7), min).Store(dst + 48);
                            Fma.MultiplyAdd(z, Vector256.ConvertToSingle(uint8), min).Store(dst + 56);
                            return result;
                        }

                    }
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

        private enum EncodingType : byte
        {
            Raw, Quantized, AllZero, AllConstant
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 7)]
        private readonly ref struct BlockAndArrayHeader

        {
            internal readonly EncodingType type;
            internal readonly float offset;
            internal readonly byte BitsAndType;

            internal readonly byte numPix;

            internal readonly byte numBits => (byte)(0b0001_1111 & BitsAndType);
            internal readonly byte numFxLen => (byte)(BitsAndType >> 6);

        }
    }
}
