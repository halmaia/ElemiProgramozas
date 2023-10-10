    [StructLayout(LayoutKind.Explicit, Size = 32)]
    public struct PointZM
    {
        [FieldOffset(0)] private Vector256<double> vector;

        public PointZM(double x, double y, double z, double m) : this() => vector = Vector256.Create(x, y, z, m);

        public PointZM(double x, double y, double z) : this(x, y, z, double.NaN) { }

        public double X
        {
            readonly get => vector[0];
            set => vector = vector.WithElement(0, value);
        }
        public double Y
        {
            readonly get => vector[1];
            set => vector = vector.WithElement(1, value);
        }
        public double Z
        {
            readonly get => vector[2];
            set => vector = vector.WithElement(2, value);
        }
        public double M
        {
            readonly get => vector[3];
            set => vector = vector.WithElement(3, value);
        }

        public static int Count => Vector256<double>.Count;
        public static PointZM Zero => (PointZM)Vector256<double>.Zero;

        public static double Dot2D(PointZM left, PointZM right) => Point.Dot2D((Point)left, (Point)right);

        public override readonly int GetHashCode() => vector.GetHashCode();

        public override readonly string ToString() => vector.ToString();
        public static implicit operator Vector256<double>(PointZM p) => Unsafe.As<PointZM, Vector256<double>>(ref p);
        public static explicit operator PointZM(Vector256<double> vector) => Unsafe.As<Vector256<double>, PointZM>(ref vector);
        public static explicit operator Point(PointZM pointZM) => (Point)Vector256.GetLower<double>(pointZM);
        public static PointZM operator +(PointZM left, PointZM right) => (PointZM)((Vector256<double>)left + right);
        public static PointZM operator -(PointZM left, PointZM right) => (PointZM)((Vector256<double>)left - right);
        public static PointZM operator *(PointZM left, PointZM right) => (PointZM)((Vector256<double>)left * right);
        public static PointZM operator /(PointZM left, PointZM right) => (PointZM)((Vector256<double>)left / right);
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct Point
    {
        [FieldOffset(0)] private Vector128<double> vector;

        public Point(double x, double y) : this() => vector = Vector128.Create(x, y);

        public double X
        {
            readonly get => vector[0];
            set => vector = vector.WithElement(0, value);
        }
        public double Y
        {
            readonly get => vector[1];
            set => vector = vector.WithElement(1, value);
        }

        public static int Count => Vector128<double>.Count;
        public static Point Zero => (Point)Vector128<double>.Zero;

        public override readonly int GetHashCode() => vector.GetHashCode();

        public override readonly string ToString() => vector.ToString();
        public static implicit operator Vector128<double>(Point p) => Unsafe.As<Point, Vector128<double>>(ref p);
        public static explicit operator Point(Vector128<double> vector) => Unsafe.As<Vector128<double>, Point>(ref vector);
        public static Point operator +(Point left, Point right) => (Point)((Vector128<double>)left + right);
        public static Point operator -(Point left, Point right) => (Point)((Vector128<double>)left - right);
        public static Point operator *(Point left, Point right) => (Point)((Vector128<double>)left * right);
        public static Point operator /(Point left, Point right) => (Point)((Vector128<double>)left / right);

        public static double Dot2D(Point left, Point right) => Vector128.Dot<double>(left , right);
    }
