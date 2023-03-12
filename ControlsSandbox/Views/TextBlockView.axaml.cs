using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System;

namespace ControlsSandbox.Views;

public partial class TextBlockView : UserControl
{
  private PixelPoint targetPosition;
  private Point lastMousePosition;
  private DispatcherTimer timer;
  private bool isDragging;

  public TextBlockView()
  {
    InitializeComponent();

    PointerPressed += OnPointerPressed;
    PointerMoved += OnPointerMoved;
    PointerReleased += OnPointerReleased;

    timer = new DispatcherTimer
    {
      Interval = TimeSpan.FromMilliseconds(10)
    };
    timer.Tick += OnTimerTick;
  }

  private void OnTimerTick(object sender, EventArgs e)
  {
    var presenter = this.FindAncestorOfType<ContentPresenter>();
    if (presenter == null)
    {
      return;
    }

    var itemsControl = this.FindAncestorOfType<ItemsControl>();
    if (itemsControl == null)
    {
      return;
    }

    if ((int)presenter.GetValue(Canvas.LeftProperty) == targetPosition.X
      && (int)presenter.GetValue(Canvas.TopProperty) == targetPosition.Y)
    {
      return;
    }

    if (targetPosition.X < 0d)
    {
      presenter.SetValue(Canvas.LeftProperty, 0d);
    }
    else if (itemsControl.Bounds.Width < targetPosition.X + presenter.Bounds.Width)
    {
      presenter.SetValue(Canvas.LeftProperty, itemsControl.Bounds.Width - presenter.Bounds.Width);
    }
    else
    {
      presenter.SetValue(Canvas.LeftProperty, targetPosition.X);
    }

    if (targetPosition.Y < 0d)
    {
      presenter.SetValue(Canvas.TopProperty, 0d);
    }
    else if (itemsControl.Bounds.Height < targetPosition.Y + presenter.Bounds.Height)
    {
      presenter.SetValue(Canvas.TopProperty, itemsControl.Bounds.Height - presenter.Bounds.Height);
    }
    else
    {
      presenter.SetValue(Canvas.TopProperty, targetPosition.Y);
    }
  }

  private void OnPointerPressed(object sender, PointerPressedEventArgs e)
  {
    if (!CanStartDrag(e))
    {
      return;
    }

    isDragging = true;

    lastMousePosition = e.GetPosition(this);
    e.Handled = true;

    timer.Start();
  }

  private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
  {
    if (!isDragging)
    {
      return;
    }

    isDragging = false;
    e.Handled = true;

    timer.Stop();
  }

  private void OnPointerMoved(object sender, PointerEventArgs e)
  {
    if (!(isDragging && CanDrag(e)))
    {
      return;
    }

    var presenter = this.FindAncestorOfType<ContentPresenter>();
    if (presenter == null)
    {
      return;
    }

    var currentMousePosition = e.GetPosition(this);
    var offset = currentMousePosition - lastMousePosition;

    var x = presenter.GetValue(Canvas.LeftProperty);
    var y = presenter.GetValue(Canvas.TopProperty);

    targetPosition = new PixelPoint((int)(x + offset.X), (int)(y + offset.Y));
    System.Diagnostics.Debug.WriteLine(targetPosition);
  }

  private bool CanStartDrag(PointerEventArgs e) => e.KeyModifiers == KeyModifiers.Control && CanDrag(e);

  private bool CanDrag(PointerEventArgs e) => e.GetCurrentPoint(this).Properties.IsLeftButtonPressed;
}