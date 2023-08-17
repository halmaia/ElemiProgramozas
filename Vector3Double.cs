    [StructLayout(LayoutKind.Explicit, Size = 256 / 8)]
    public struct Vector3Double : IEquatable<Vector3Double>,
        ITuple
    {
        [FieldOffset(0)] private Vector256<double> vector;
        [field: FieldOffset(0)] public double X { readonly get; set; }
        [field: FieldOffset(8)] public double Y { readonly get; set; }
        [field: FieldOffset(16)] public double Z { readonly get; set; }

        public Vector3Double(double value) => vector = Vector256.Create(value, value, value, 0);

        public Vector3Double(double x, double y, double z) => vector = Vector256.Create(x, y, z, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3Double AsVector3Double(Vector256<double> vector) =>
            Unsafe.As<Vector256<double>, Vector3Double>(ref vector);

        public static Vector3Double Zero => AsVector3Double(Vector256<double>.Zero);
        public static Vector3Double UnitX => AsVector3Double(Vector256.Create(1d, 0, 0, 0));
        public static Vector3Double UnitY => AsVector3Double(Vector256.Create(0d, 1, 0, 0));
        public static Vector3Double UnitZ => AsVector3Double(Vector256.Create(0d, 0, 1, 0));

        readonly int ITuple.Length => 3;
        readonly object? ITuple.this[int index] => this[index];

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
            AsVector3Double(left.vector + right.vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double operator -(Vector3Double left, Vector3Double right) =>
            AsVector3Double(left.vector - right.vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double operator -(Vector3Double unary) =>
            AsVector3Double(-unary.vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double operator *(Vector3Double left, Vector3Double right) =>
            AsVector3Double(left.vector * right.vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double operator *(Vector3Double left, double right) =>
           AsVector3Double(left.vector * right);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double operator *(double left, Vector3Double right) =>
           AsVector3Double(left * right.vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double operator /(Vector3Double left, Vector3Double right) =>
            AsVector3Double(left.vector / right.vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double operator /(Vector3Double left, double right) =>
           AsVector3Double(left.vector / right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3Double left, Vector3Double right) =>
          left.vector == right.vector;


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3Double left, Vector3Double right) =>
         left.vector == right.vector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly bool Equals([NotNullWhen(true)] object? obj) => (obj is Vector3Double other) && Equals(other);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly bool Equals(Vector3Double other) => this == other;

        public override readonly int GetHashCode()
        {
            HashCode hashCode = default;
            hashCode.Add(X); hashCode.Add(Y); hashCode.Add(Z);
            return hashCode.ToHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly double Dot(Vector3Double other) =>
            Vector256.Dot(this.vector, other.vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly double LengthSquared() =>
            Vector256.Dot(vector, vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly double Length() =>
            double.Sqrt(LengthSquared());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double Normalize(Vector3Double value) =>
            value / value.Length();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Vector256<double> AsVector256() =>
            vector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ValueTuple<double,double,double> AsTuple() =>
            (X, Y, Z);

        public readonly void Deconstruct(out double x, out double y, out double z)
        {
            x = X; y = Y; z = Z;
        }

        public static bool IsHardwareAccelerated
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector256.IsHardwareAccelerated;
        }
    }
