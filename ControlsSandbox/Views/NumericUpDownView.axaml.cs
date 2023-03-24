using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ControlsSandbox.Views;

public partial class NumericUpDownView : UserControl
{
  public NumericUpDownView()
  {
    InitializeComponent();
  }

  private void InitializeComponent()
  {
    AvaloniaXamlLoader.Load(this);

    var editor = this.FindControl<TextBox>(nameof(Editor));
    editor.AddHandler(KeyDownEvent, OnTextInput, RoutingStrategies.Tunnel);
  }

  private void OnTextInput(object sender, KeyEventArgs e)
    => e.Handled = !CanUse(e);

  private bool CanUse(KeyEventArgs e)
  {
    switch (e.Key)
    {
      case Key.D0 or Key.NumPad0:
      case Key.D1 or Key.NumPad1:
      case Key.D2 or Key.NumPad2:
      case Key.D3 or Key.NumPad3:
      case Key.D4 or Key.NumPad4:
      case Key.D5 or Key.NumPad5:
      case Key.D6 or Key.NumPad6:
      case Key.D7 or Key.NumPad7:
      case Key.D8 or Key.NumPad8:
      case Key.D9 or Key.NumPad9:
      case Key.Left or Key.Right or Key.Up or Key.Down:
      case Key.Home or Key.End:
      case Key.Delete or Key.Back:
        return true;
      case Key.A or Key.C or Key.V:
        return e.KeyModifiers == KeyModifiers.Control;
      default:
        return false;
    }
  }
}