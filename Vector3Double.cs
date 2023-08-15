    [StructLayout(LayoutKind.Explicit, Size = 256 / 8)]
    public struct Vector3Double
    {
        [FieldOffset(0)] private Vector256<double> vector;

        public Vector3Double(double value) => vector = Vector256.Create(value);

        public Vector3Double(double x, double y, double z) : this() => vector = Vector256.Create(x, y, z, 0);

        public static Vector3Double Zero => Unsafe.BitCast<Vector256<double>, Vector3Double>(Vector256<double>.Zero);

        public static Vector3Double UnitX => Unsafe.BitCast<Vector256<double>, Vector3Double>(Vector256.Create(1d, 0, 0, 0));
        public static Vector3Double UnitY => Unsafe.BitCast<Vector256<double>, Vector3Double>(Vector256.Create(0d, 1, 0, 0));
        public static Vector3Double UnitZ => Unsafe.BitCast<Vector256<double>, Vector3Double>(Vector256.Create(0d, 0, 1, 0));

        public double this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get
            {
                if ((uint)(index) >= 3u)
                {
                    ThrowIndexException();

                    static void ThrowIndexException()
                    {
                        throw new ArgumentOutOfRangeException(nameof(index));
                    }
                }

                ref double address = ref Unsafe.As<Vector3Double, double>(ref Unsafe.AsRef(in this));
                return Unsafe.Add(ref address, index);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double operator +(Vector3Double left, Vector3Double right) =>
            Unsafe.BitCast<Vector256<double>, Vector3Double>(left.vector + right.vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double operator -(Vector3Double left, Vector3Double right) =>
            Unsafe.BitCast<Vector256<double>, Vector3Double>(left.vector - right.vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double operator *(Vector3Double left, Vector3Double right) =>
            Unsafe.BitCast<Vector256<double>, Vector3Double>(left.vector * right.vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double operator /(Vector3Double left, Vector3Double right) =>
            Unsafe.BitCast<Vector256<double>, Vector3Double>(left.vector / right.vector);

        [field: FieldOffset(0)] public double X { readonly get; set; }
        [field: FieldOffset(8)] public double Y { readonly get; set; }
        [field: FieldOffset(16)] public double Z { readonly get; set; }
    }
