using System;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            GetSlopeAndAspect(new double[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }, 4, 1, out double[] s, out double[] a);

        }
        private static void GetSlopeAndAspect(double[] matrix, int stride, double cellsize, out double[] slope, out double[] aspect)
        {
            if (matrix == null) throw new ArgumentNullException(nameof(matrix));

            int matrix_length = matrix.Length;
            if (matrix_length < 9) throw new ArgumentOutOfRangeException(nameof(matrix));

            if (stride < 3 ||
                stride >= matrix_length ||
                matrix_length % stride != 0) throw new ArgumentOutOfRangeException(nameof(stride));

            if (double.IsNaN(cellsize) ||
                cellsize <= 0) throw new ArgumentOutOfRangeException(nameof(cellsize));

            int output_length = (matrix_length / stride - 2) * (stride - 2);
            slope = new double[output_length];
            aspect = new double[output_length];
            int n = 0;

            double divider = 8d * cellsize;
            int b = -stride, a = b - 1, c = b + 1;
            int g = stride - 1, i = stride + 1;

            for (int y = stride; y < matrix_length - stride; y += stride)
                for (int x = 1; x < g; x++)
                {
                    int index = x + y;
                    double val_i = matrix[index + i];
                    double val_a = matrix[index + a];
                    double val_c = matrix[index + c];
                    double val_g = matrix[index + g];
                    double dx = (val_c + 2d * matrix[index + 1] + val_i - (val_a + 2d * matrix[index - 1] + val_g)) / divider;
                    double dy = (val_g + 2d * matrix[index + stride] + val_i - (val_a + 2d * matrix[index - stride] + val_c)) / divider;

                    slope[n] = Math.Sqrt(dy * dy + dx * dx);
                    double imed_aspect = 0d;

                    if (dx != 0d)
                    {
                        imed_aspect = Math.Atan2(dy, -dx);
                        if (imed_aspect < 0d) imed_aspect += 2d * Math.PI;
                    }
                    else if (dy > 0d) imed_aspect = Math.PI / 2d;
                    else if (dy < 0d) imed_aspect = 1.5d * Math.PI;

                    aspect[n++] = imed_aspect;
                }
        }
    }
}
