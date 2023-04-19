using System.Runtime.InteropServices;

namespace ConsoleApp2
{
    internal class Program
    {
        private static (float min, float max) GetMinMax(ReadOnlySpan<float> floatArray)
        {
            float min = floatArray[0], max = min;
            for (int i = 0, len = floatArray.Length; i < len;)
            {
                float current = floatArray[i++];
                if (current > max) max = current; else if (current < min) min = current;
            }
            return (min, max);
        }

        private static int GetNumberOfBits(float min, float max, float maxError) =>
            (int)float.Ceiling(float.Log2(((max - min) / (2f * maxError))));

        private static ReadOnlySpan<uint> Encode(ReadOnlySpan<float> fromArray, float min, float maxError)
        {
            float n = 1f / (2f * maxError);
            int len = fromArray.Length;
            uint[] array = GC.AllocateUninitializedArray<uint>(len);
            for (int i = 0; i < len; i++)
            {
                array[i] = (uint)float.FusedMultiplyAdd(fromArray[i] - min, n, .5f);
            }
            return array;
        }

        private static ReadOnlySpan<float> Decode(ReadOnlySpan<uint> fromArray, float min, float maxError)
        {
            float n = 2f * maxError;
            int len = fromArray.Length;
            float[] array = GC.AllocateUninitializedArray<float>(len);
            for (int i = 0; i < len; i++)
            {
                array[i] = float.FusedMultiplyAdd(n, fromArray[i], min);
            }
            return array;
        }



        private static int GetBlobSize(int numberOfBits, int originalLength) => 4 * (int)float.Ceiling(originalLength * numberOfBits / 32f);
        private static ReadOnlySpan<float> GetFloatSpan(string path) => MemoryMarshal.Cast<byte, float>(File.ReadAllBytes(path));
        static void Main()
        {
            ReadOnlySpan<float> fromArray = GetFloatSpan(@"F:\NetFlux.flt");
            (float min, float max) = GetMinMax(fromArray);
            int numberOfBits = GetNumberOfBits(min, max, .1f);
            int blobSize = GetBlobSize(numberOfBits, fromArray.Length);
            ReadOnlySpan<uint> unstuffedArray = Encode(fromArray, min, .1f);
            ReadOnlySpan<byte> stuffedArray = BitStuffer. Stuff(unstuffedArray, blobSize, numberOfBits);


            //ReadOnlySpan<uint> deStuffedArray = UnStuff(stuffedArray, unstuffedArray.Length, numberOfBits);

            //var x = Decode(deStuffedArray, min, .1f);

        }
    }

    public static class BitStuffer
    {
        public static unsafe ReadOnlySpan<byte> Stuff(ReadOnlySpan<uint> unstuffedArray, int blobSize, int numBits)
        {
            Span<uint> stuffedArray = GC.AllocateArray<uint>((int)(blobSize / 4d + .5));
            fixed (uint* dst = stuffedArray)
            fixed (uint* src = unstuffedArray)
            {
                uint* srcPtr = src, dstPtr = dst;
                for (int i = 0, bitPos = 0, len = unstuffedArray.Length; i < len; i++)
                {
                    if (32 - bitPos >= numBits)    // 0 < numBits < 32
                    {
                        *dstPtr |= (*srcPtr++) << bitPos;
                        bitPos += numBits;
                        if (bitPos == 32)    // shift >= 32 is undefined
                        {
                            dstPtr++;
                            bitPos = 0;
                        }
                    }
                    else
                    {
                        *dstPtr++ |= (*srcPtr) << bitPos;
                        *dstPtr |= (*srcPtr++) >> (32 - bitPos);    // bitPos > 0 here, always
                        bitPos += numBits - 32;
                    }
                }
            }
            return MemoryMarshal.Cast<uint, byte>(stuffedArray)[..blobSize];
        }

        public static unsafe ReadOnlySpan<uint> UnStuff(ReadOnlySpan<byte> stuffedArray, int numElements, int numBits)
        {
            Span<uint> unstuffedArray = GC.AllocateArray<uint>(numElements);
            ReadOnlySpan<uint> srcSpan = MemoryMarshal.Cast<byte, uint>(stuffedArray);

            fixed (uint* dst = unstuffedArray)
            fixed (uint* src = srcSpan)
            {
                uint* srcPtr = src, dstPtr = dst;
                int nb = 32 - numBits;
                for (int i = 0, bitPos = 0; i < numElements; i++)
                {
                    if (nb - bitPos >= 0)
                    {
                        *dstPtr++ = ((*srcPtr) << (nb - bitPos)) >> nb;
                        bitPos += numBits;
                        if (bitPos == 32)    // shift >= 32 is undefined
                        {
                            srcPtr++;
                            bitPos = 0;
                        }
                    }
                    else
                    {
                        *dstPtr = (*srcPtr++) >> bitPos;
                        *dstPtr++ |= ((*srcPtr) << (64 - numBits - bitPos)) >> nb;
                        bitPos -= nb;
                    }
                }
            }
            return unstuffedArray;
        }
    }
}
