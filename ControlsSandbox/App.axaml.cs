using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ControlsSandbox.ViewModels;
using ControlsSandbox.Views;

namespace ControlsSandbox;

public partial class App : Application
{
  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted()
  {
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
    {
      desktop.MainWindow = new MainWindow
      {
        DataContext = new MainWindowViewModel(),
      };

      MeasurementConverter.InitializeFrom(desktop.MainWindow.PlatformImpl);
    }

    base.OnFrameworkInitializationCompleted();
  }
}
