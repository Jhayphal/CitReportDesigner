using ReactiveUI;

namespace ControlsSandbox.ViewModels
{
  public class ViewModelBase : ReactiveObject
  {
    protected const double MillimetersInInch = 25.4f;

    protected static readonly double DPI = 96d;
    
    internal static double ScaleFactor = 1d;

    protected static double MillimetersToPixels(double value)
      => ((value / MillimetersInInch) * DPI) / ScaleFactor;
  }
}
