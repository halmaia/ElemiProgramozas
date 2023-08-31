using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LERC_CS
{
    internal static class Program
    {
        static void Main()
        {
            var s = Lerc.ComputeCompressedSize<byte>(new ReadOnlySpan<byte>(new byte[4] { 10, 20, 30, 40 }),
                1, 2, 2, 1, 0, null, 5);
        }
    }

    public static class Lerc
    {
        private const string LercDLL = @"Lerc.dll";

        #region Error Handling
        [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
        private static void DetectError(ErrCode errCode)
        {
            if (errCode is not ErrCode.OK)
                ThrowAPIError(errCode);

            [DoesNotReturn, SkipLocalsInit]
            static void ThrowAPIError(ErrCode errCode) =>
                throw errCode switch
                {
                    ErrCode.Failed =>
                        new Exception("Operation failed."),
                    ErrCode.WrongParam =>
                        new ArgumentException("Wrong parameter."),
                    ErrCode.BufferTooSmall =>
                        new ArgumentException("Buffer too small."),
                    ErrCode.NaN =>
                         new ArgumentException("There is NaN in the provided dataset."),
                    ErrCode.HasNoData =>
                         new ArgumentException("NoData error."),
                    _ => new NotImplementedException()
                };
        }

        #endregion Error Handling

        [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
        private static DataType GetDataType(Type type)
        {
            if (type == typeof(sbyte)) return DataType.SByte;
            if (type == typeof(byte)) return DataType.Byte;
            if (type == typeof(short)) return DataType.Short;
            if (type == typeof(ushort)) return DataType.UShort;
            if (type == typeof(int)) return DataType.Int;
            if (type == typeof(uint)) return DataType.UInt;
            if (type == typeof(float)) return DataType.Float;
            if (type == typeof(double)) return DataType.Double;
            throw new NotSupportedException("Invalid data type.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
        public static unsafe uint ComputeCompressedSize<T>(
            ReadOnlySpan<T> data,
            int nDepth,
            int nCols,
            int nRows,
            int nBands,
            int nMasks,
            ReadOnlySpan<byte> validBytes,
            double maxError) where T : unmanaged
        {
            fixed (void* pData = data)
            fixed (byte* pValidBytes = validBytes)
                return ComputeCompressedSize(pData,
                     GetDataType(typeof(T)),
                     nDepth, nCols, nRows, nBands, nMasks,
                     pValidBytes, maxError);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining), SkipLocalsInit]
        public static unsafe uint ComputeCompressedSize(
             void* pData,                 // raw image data, row by row, band by band
                                              DataType dataType,             // char = 0, uchar = 1, short = 2, ushort = 3, int = 4, uint = 5, float = 6, double = 7
                                              int nDepth,                        // number of values per pixel (e.g., 3 for RGB, data is stored as [RGB, RGB, ...])
                                              int nCols,                         // number of columns
                                              int nRows,                         // number of rows
                                              int nBands,                        // number of bands (e.g., 3 for [RRRR ..., GGGG ..., BBBB ...])
                                              int nMasks,                        // 0 - all valid, 1 - same mask for all bands, nBands - masks can differ between bands
                                              byte* pValidBytes,  // nullptr if all pixels are valid; otherwise 1 byte per pixel (1 = valid, 0 = invalid)
                                              double maxZErr)          // size of outgoing Lerc blob
        {

            Unsafe.SkipInit(out uint NumBytes);
            DetectError(lerc_computeCompressedSize(pData, dataType, nDepth, nCols, nRows, nBands, nMasks, pValidBytes, maxZErr, &NumBytes));
            return NumBytes;

            [DllImport(LercDLL, BestFitMapping = false, CallingConvention = CallingConvention.StdCall,
                       EntryPoint = nameof(lerc_computeCompressedSize), ExactSpelling = true,
                       PreserveSig = true, SetLastError = false)]
            static unsafe extern ErrCode lerc_computeCompressedSize(
                                                  void* pData,                 // raw image data, row by row, band by band
                                                  DataType dataType,             // char = 0, uchar = 1, short = 2, ushort = 3, int = 4, uint = 5, float = 6, double = 7
                                                  int nDepth,                        // number of values per pixel (e.g., 3 for RGB, data is stored as [RGB, RGB, ...])
                                                  int nCols,                         // number of columns
                                                  int nRows,                         // number of rows
                                                  int nBands,                        // number of bands (e.g., 3 for [RRRR ..., GGGG ..., BBBB ...])
                                                  int nMasks,                        // 0 - all valid, 1 - same mask for all bands, nBands - masks can differ between bands
                                                  byte* pValidBytes,  // nullptr if all pixels are valid; otherwise 1 byte per pixel (1 = valid, 0 = invalid)
                                                  double maxZErr,                    // max coding error per pixel, defines the precision
                                                  uint* numBytes);           // size of outgoing Lerc blob
        }



    }

    //public static Span<byte> Encode<T>
    //    (
    //    ReadOnlySpan<T> data,
    //    int nDepth,
    //    int nCols,
    //    int nRows,
    //    int nBands,
    //    int nMasks,
    //    ReadOnlySpan<Byte> validBytes

    //    double maxError
    //    )
    //{



    //}



    public enum ErrCode : int { OK, Failed, WrongParam, BufferTooSmall, NaN, HasNoData };
    public enum DataType { SByte, Byte, Short, UShort, Int, UInt, Float, Double };

    [StructLayout(LayoutKind.Sequential, Size = 11 * sizeof(int))]
    public readonly struct Info
    {
        public readonly int Version { get; }
        public readonly DataType DataType { get; }
        public readonly int NDim { get; }
        public readonly int NCols { get; }
        public readonly int NRows { get; }
        public readonly int NBands { get; }
        public readonly int NValidPixels { get; }
        public readonly int BlobSize { get; }
        public readonly int NMasks { get; }
        public readonly int NDepth { get; }
        public readonly int NUsesNoDataValue { get; }
    }
}
