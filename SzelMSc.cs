        public static (double avg_Speed, double avg_Dir) GetAverageWindSpeedAndDirection(
            Span<double> speeds, Span<double> directions)
        {
            int len = speeds.Length;
            if (len < 1) throw new ArgumentOutOfRangeException(nameof(speeds), "Speed is empty.");
            if (len != directions.Length) throw new ArgumentException("The number of element is different.");

            int i = 0;
            double avg_u = 0, avg_v = 0;
            do
            {

                (double u, double v) tuple = double.SinCos(double.DegreesToRadians(directions[i]));
                double speed = -speeds[i++];
                avg_u = double.FusedMultiplyAdd(speed, tuple.u, avg_u);
                avg_v = double.FusedMultiplyAdd(speed, tuple.v, avg_v);
            } while (i != len);
            avg_u /= len;
            avg_v /= len;
            double avg_dir = double.RadiansToDegrees(double.Atan2(avg_u, avg_v));
            if (avg_dir < 180) avg_dir += 180; else if (avg_dir > 180) avg_dir -= 180;
            return (double.Hypot(avg_u, avg_v), avg_dir);
        }
