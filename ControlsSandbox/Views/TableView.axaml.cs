using Avalonia.Controls;
using Avalonia.Input;
using ControlsSandbox.Behaviors;

namespace ControlsSandbox.Views;

public partial class TableView : UserControl
{
#pragma warning disable IDE0052 // Remove unread private members
  private readonly ControlBehavior[] behaviors;
#pragma warning restore IDE0052 // Remove unread private members

  public TableView()
  {
    InitializeComponent();

    behaviors = new ControlBehavior[]
    {
      new DraggableControlBehavior<Canvas>(this, KeyModifiers.Control)
    };
  }
}