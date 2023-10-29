using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

internal class Program
{
    private unsafe static void Main(string[] args)
    {
        const string path = @"F:\lerc8_8.lrc";
        MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(path, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
        MemoryMappedViewAccessor acc = mmf.CreateViewAccessor(0L, 0L, MemoryMappedFileAccess.Read);
        var handle = acc.SafeMemoryMappedViewHandle;
        byte* ptr = null;
        handle.AcquirePointer(ref ptr);

        if (new string((sbyte*)ptr, 0, 6) != @"Lerc2 ")
            throw new InvalidDataException("Invalid file!");

        switch (*(int*)(ptr += 6))
        {
            case 6:
                HeaderInfo headerInfo = *(HeaderInfo*)(ptr += 4);
                ReadOnlySpan<float> minMaxRanges = new(ptr+=sizeof(HeaderInfo),3);
                ReadOnlySpan<byte> array = new(ptr+3*sizeof(float), (headerInfo.blobSize-6-4-sizeof(HeaderInfo)-4-4-4));
                break;
            default:
                break;
        }




        handle.ReleasePointer();
        ptr = null;
        handle.Dispose();
        acc.Dispose();
        mmf.Dispose();
    }
}

[StructLayout(LayoutKind.Sequential)]
public readonly struct HeaderInfo
{
    public readonly uint checksum;
    public readonly int nRows;
    public readonly int nCols;
    public readonly int nDepth;
    public readonly int numValidPixel;
    public readonly int microBlockSize;
    public readonly int blobSize;
    public readonly int dt;

    public readonly int nBlobsMore;         // for multi-band Lerc blob, how many more blobs or bands appended to this one

    public readonly byte bPassNoDataValues;  // 1 - pass noData values to decoder, 0 - don't pass, ignore
    public readonly byte bIsInt;    // 1 - float or double data is all integer numbers, 0 - not
    public readonly byte bReserved3;
    public readonly byte bReserved4;



    public readonly double maxZError;
    public readonly double zMin;    // if nDepth > 1, this is the overall range
    public readonly double zMax;
    public readonly double noDataVal;      // temp noData value used for nDepth > 1 if bit mask cannot cover it
    public readonly double noDataValOrig;  // orig noData value to map to in decode
}
