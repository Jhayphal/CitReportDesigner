using ReactiveUI;

namespace ControlsSandbox.ViewModels
{
  public class ViewModelBase : ReactiveObject
  {
    protected const float SkiaDPI = 96f;
    protected const float MillimetersInInch = 25.4f;

    protected static float MillimetersToPixels(float value) => (value / MillimetersInInch) * SkiaDPI;
  }
}
