using Avalonia.Controls;
using ControlsSandbox.Extensions;
using ControlsSandbox.ViewModels;

namespace ControlsSandbox.Views
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      ViewModelBase.ScaleFactor = this.GetActiveScreen().PixelDensity;
    }
  }
}
