namespace ControlsSandbox
{
  public static class MeasurementConverter
  {
    private const double PrettyFactor = 2.5d;

    private const double MillimetersInInch = 25.4f;

    private static readonly double DPI = 96d;

    internal static double ScaleFactor = 1d;

    public static double MillimetersToPixels(double value)
      => (((value / MillimetersInInch) * DPI) / ScaleFactor) * PrettyFactor;
  }
}
