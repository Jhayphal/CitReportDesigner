using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using ControlsSandbox.Behaviors;

namespace ControlsSandbox.Views;

public partial class CellView : UserControl
{
#pragma warning disable IDE0052 // Remove unread private members
  private readonly ControlBehavior[] behaviors;
#pragma warning restore IDE0052 // Remove unread private members

  public CellView()
  {
    InitializeComponent();

    behaviors = new ControlBehavior[]
    {
      new ResizableCellBehavior<Canvas>(this, KeyModifiers.Alt, minimalSize: new Size(10, 10), findLastAncestor: true)
    };
  }
}