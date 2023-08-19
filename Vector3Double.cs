    [StructLayout(LayoutKind.Explicit, Size = 256 / 8)]
    [DebuggerDisplay($"{{{nameof(ToString)}(),nq}}")]
    public struct Vector3Double : IEquatable<Vector3Double>,
        ITuple
    {
        [FieldOffset(0)] private readonly Vector256<double> vector;
        [field: FieldOffset(0)] public double X { readonly get; set; }
        [field: FieldOffset(8)] public double Y { readonly get; set; }
        [field: FieldOffset(16)] public double Z { readonly get; set; }
        public Vector3Double(double value) => this = AsVector3Double(Vector256.Create(value, value, value, 0));
        public Vector3Double(double x, double y, double z) => this = AsVector3Double(Vector256.Create(x, y, z, 0));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3Double AsVector3Double(Vector256<double> vector) =>
            Unsafe.As<Vector256<double>, Vector3Double>(ref vector);

        public static Vector3Double Zero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AsVector3Double(Vector256<double>.Zero);
        }

        public static Vector3Double One
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AsVector3Double(Vector256.Create(1d, 1d, 1d, 0d));
        }

        public static Vector3Double UnitX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AsVector3Double(Vector256.Create(1d, 0, 0, 0));
        }

        public static Vector3Double UnitY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AsVector3Double(Vector256.Create(0d, 1, 0, 0));
        }

        public static Vector3Double UnitZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => AsVector3Double(Vector256.Create(0d, 0, 1, 0));
        }

        readonly int ITuple.Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 3;
        }

        readonly object? ITuple.this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this[index];
        }
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
         left.vector != right.vector;

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
        public static double Dot(Vector3Double left, Vector3Double right) =>
            Vector256.Dot(left.vector, right.vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly double Dot(Vector3Double other) =>
            Vector256.Dot(vector, other.vector);

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
        public readonly Vector3Double Normalize() =>
             this / Length();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double DistanceSquared(Vector3Double left, Vector3Double right) =>
            (left - right).LengthSquared();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly double DistanceSquared(Vector3Double other) =>
            (this - other).LengthSquared();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Distance(Vector3Double left, Vector3Double right) =>
            double.Sqrt(DistanceSquared(left, right));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly double Distance(Vector3Double other) =>
           double.Sqrt(DistanceSquared(other));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double Min(Vector3Double left, Vector3Double right) =>
            AsVector3Double(Vector256.Min(left.vector, right.vector));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double Max(Vector3Double left, Vector3Double right) =>
            AsVector3Double(Vector256.Max(left.vector, right.vector));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double Clamp(Vector3Double value, Vector3Double min, Vector3Double max) =>
            Min(Max(value, min), max);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double Lerp(Vector3Double value1, Vector3Double value2, double amount) =>
            ((1.0d - amount) * value1) + (value2 * amount);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Vector3Double Sqrt() =>
            AsVector3Double(Vector256.Sqrt(vector));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3Double Sqrt(Vector3Double vector) =>
            AsVector3Double(Vector256.Sqrt(vector.vector));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Vector256<double> AsVector256Double() =>
            vector;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Vector256<long> AsVector256Long() =>
             Vector256.ConvertToInt64(vector);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly ValueTuple<double, double, double> AsTuple() =>
            (X, Y, Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void Deconstruct(out double x, out double y, out double z)
        {
            x = X; y = Y; z = Z;
        }

        public static bool IsHardwareAccelerated
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Vector256.IsHardwareAccelerated;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override readonly string ToString() => 
            ToString("G", CultureInfo.CurrentCulture);
        public readonly string ToString([StringSyntax(StringSyntaxAttribute.NumericFormat)] string? format, IFormatProvider? formatProvider)
        {
            string separator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
            return $"<{X.ToString(format, formatProvider)}{separator} {Y.ToString(format, formatProvider)}{separator} {Z.ToString(format, formatProvider)}>";
        }
    }
