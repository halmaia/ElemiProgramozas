internal class KahanBabuska
    {
        private double sum, com;

        internal KahanBabuska(double startValue = 0) =>
            sum = startValue;
        [SkipLocalsInit]
        internal void Add(double value)
        {
            double sum = this.sum,
                   n = sum + value;
            com += Math.Abs(sum) >= Math.Abs(value) ?
                (sum - n) + value :
                (value - n) + sum;
            this.sum = n;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Reset()
        {
            com = 0; sum = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal double GetResult() =>
            sum + com;
    }
