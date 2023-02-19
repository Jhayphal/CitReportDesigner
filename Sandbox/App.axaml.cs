using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Sandbox.Services;
using Sandbox.ViewModels;
using Sandbox.Views;

namespace Sandbox
{
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
        var db = new Database();

        desktop.MainWindow = new MainWindow
        {
          DataContext = new MainWindowViewModel(db),
        };
      }

      base.OnFrameworkInitializationCompleted();
    }
  }
}
