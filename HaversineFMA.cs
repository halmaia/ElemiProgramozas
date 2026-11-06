private static double Distance(double φ1​, double φ2, double λ1​, double λ2) // Haversine
{
  double a = double.FusedMultiplyAdd(dcos(φ1) * dcos(φ2), dhav(λ2 - λ1), dhav(φ2 - φ1));
  return (2d * 6371000d) * double.Atan2(double.Sqrt(a), double.Sqrt(1d - a));

  static double dcos(double x) =>
     double.Cos(double.DegreesToRadians(x));

  static double dhav(double x)
  {
    double s = double.Sin((double.Pi / 360d) * x);
    return s * s;
    }
}
