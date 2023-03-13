using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using System;

namespace ControlsSandbox.Behaviors;

public sealed class ResizableControlBehavior : ControlBehavior
{
  private readonly UserControl targetControl;
  private readonly DispatcherTimer timer;

  private (HorizontalAlignment, VerticalAlignment) resizeDirection;
  private PixelPoint targetOffset;
  private Point lastMousePosition;
  private bool isResizing;
  private IControlBounds viewModel;

  public ResizableControlBehavior(UserControl userControl)
  {
    targetControl = userControl;

    targetControl.PointerPressed += OnPointerPressed;
    targetControl.PointerMoved += OnPointerMoved;
    targetControl.PointerReleased += OnPointerReleased;

    timer = new DispatcherTimer
    {
      Interval = TimeSpan.FromMilliseconds(10)
    };

    timer.Tick += OnTimerTick;
  }

  private void OnPointerPressed(object sender, PointerPressedEventArgs e)
  {
    if (!CanStartResize(e))
    {
      return;
    }

    viewModel = targetControl.DataContext as IControlBounds;
    if (viewModel == null)
    {
      return;
    }

    if (viewModel.SizeUnit == ReportSizeUnit.Millimeter)
    {
      viewModel = new MillimetersToPixelsBoundsAdapter(viewModel);
    }

    lastMousePosition = e.GetPosition(targetControl);
    targetOffset = new PixelPoint();

    isResizing = true;
    e.Handled = true;

    timer.Start();
  }

  private void OnPointerMoved(object sender, PointerEventArgs e)
  {
    if (!(isResizing && CanResize(e)))
    {
      if (e.KeyModifiers == KeyModifiers.Alt)
      {
        targetControl.Cursor = GetCursorForDirection(GetDirection(e.GetPosition(targetControl)));
      }

      return;
    }

    var newMousePosition = e.GetPosition(targetControl);
    var offset = lastMousePosition - newMousePosition;
    lastMousePosition = newMousePosition;

    targetOffset = new PixelPoint((int)offset.X, (int)offset.Y);
  }

  private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
  {
    if (!isResizing)
    {
      return;
    }

    isResizing = false;
    targetControl.Cursor = Cursor.Default;
    e.Handled = true;

    timer.Stop();
  }

  private void OnTimerTick(object sender, EventArgs e)
  {
    if (targetOffset.X == 0 && targetOffset.Y == 0)
    {
      return;
    }

    var x = viewModel.X;
    var y = viewModel.Y;

    if (resizeDirection.Item1 == HorizontalAlignment.Left)
    {
      viewModel.X = x - targetOffset.X;
      viewModel.Width += targetOffset.X;
    }
    else if (resizeDirection.Item1 == HorizontalAlignment.Right)
    {
      viewModel.Width -= targetOffset.X;
    }

    if (resizeDirection.Item2 == VerticalAlignment.Top)
    {
      viewModel.Y = y - targetOffset.Y;
      viewModel.Height += targetOffset.Y;
    }
    else if (resizeDirection.Item2 == VerticalAlignment.Bottom)
    {
      viewModel.Height -= targetOffset.Y;
    }

    targetOffset = new PixelPoint();
  }

  private bool CanStartResize(PointerEventArgs e)
  {
    if (e.KeyModifiers == KeyModifiers.Alt && CanResize(e))
    {
      resizeDirection = GetDirection(e.GetPosition(targetControl));

      return !(resizeDirection.Item1 == HorizontalAlignment.Center
        && resizeDirection.Item2 == VerticalAlignment.Center);
    }

    return false;
  }

  private bool CanResize(PointerEventArgs e) => e.GetCurrentPoint(targetControl).Properties.IsLeftButtonPressed;

  private (HorizontalAlignment, VerticalAlignment) GetDirection(Point point)
  {
    var cornerLength = targetControl.Bounds.Height / 3d;

    var leftX = cornerLength;
    var rightX = targetControl.Bounds.Width - cornerLength;

    HorizontalAlignment horizontal = point.X <= leftX
      ? HorizontalAlignment.Left
      : point.X >= rightX
        ? HorizontalAlignment.Right
        : HorizontalAlignment.Center;

    var topY = cornerLength;
    var bottomY = targetControl.Bounds.Height - cornerLength;

    VerticalAlignment vertical = point.Y <= topY
      ? VerticalAlignment.Top
      : point.Y >= bottomY
        ? VerticalAlignment.Bottom
        : VerticalAlignment.Center;

    return (horizontal, vertical);
  }

  private Cursor GetCursorForDirection((HorizontalAlignment, VerticalAlignment) direction)
  {
    if (direction.Item1 == HorizontalAlignment.Center && direction.Item2 == VerticalAlignment.Center)
    {
      return Cursor.Default;
    }

    if (direction.Item1 == HorizontalAlignment.Stretch || direction.Item2 == VerticalAlignment.Stretch)
    {
      throw new InvalidOperationException("Stretch unexpected.");
    }

    return direction.Item1 switch
    {
      HorizontalAlignment.Left => direction.Item2 == VerticalAlignment.Top
        ? new Cursor(StandardCursorType.TopLeftCorner)
        : direction.Item2 == VerticalAlignment.Center
          ? new Cursor(StandardCursorType.LeftSide)
          : new Cursor(StandardCursorType.BottomLeftCorner),
      HorizontalAlignment.Center => direction.Item2 == VerticalAlignment.Top
        ? new Cursor(StandardCursorType.TopSide)
        : new Cursor(StandardCursorType.BottomSide),
      HorizontalAlignment.Right => direction.Item2 == VerticalAlignment.Top
        ? new Cursor(StandardCursorType.TopRightCorner)
        : direction.Item2 == VerticalAlignment.Center
          ? new Cursor(StandardCursorType.RightSide)
          : new Cursor(StandardCursorType.BottomRightCorner),
      _ => throw new InvalidOperationException($"{direction.Item1}:{direction.Item2} unexpected."),
    };
  }
}