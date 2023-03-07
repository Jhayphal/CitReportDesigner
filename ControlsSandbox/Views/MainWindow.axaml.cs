using Avalonia.Controls;
using ControlsSandbox.Extensions;

namespace ControlsSandbox.Views
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      MeasurementConverter.ScaleFactor = this.GetActiveScreen().PixelDensity;
    }
  }
}
