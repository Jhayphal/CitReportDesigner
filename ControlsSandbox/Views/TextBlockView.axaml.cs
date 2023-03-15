using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using ControlsSandbox.Behaviors;

namespace ControlsSandbox.Views;

public partial class TextBlockView : UserControl
{
#pragma warning disable IDE0052 // Remove unread private members
  private readonly ControlBehavior[] behaviors;
#pragma warning restore IDE0052 // Remove unread private members

  public TextBlockView()
  {
    InitializeComponent();

    behaviors = new ControlBehavior[]
    {
      new DraggableControlBehavior<Canvas>(this, KeyModifiers.Control),
      new ResizableControlBehavior<Canvas>(this, KeyModifiers.Alt, new Size(10, 10))
    };
  }
}