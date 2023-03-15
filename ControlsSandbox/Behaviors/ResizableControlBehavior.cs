using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlsSandbox.Behaviors;

public sealed class ResizableControlBehavior : ControlBehavior
{
  private readonly UserControl targetControl;
  private readonly DispatcherTimer timer;

  private (HorizontalAlignment Horizontal, VerticalAlignment Vertical) resizeDirection;
  private PixelPoint targetOffset;
  private Point lastMousePosition;
  private bool isResizing;
  private IControlBounds viewModel;

  private readonly Dictionary<StandardCursorType, Cursor> cursors = new()
  {
    { StandardCursorType.TopLeftCorner, new Cursor(StandardCursorType.TopLeftCorner) },
    { StandardCursorType.LeftSide, new Cursor(StandardCursorType.LeftSide) },
    { StandardCursorType.BottomLeftCorner, new Cursor(StandardCursorType.BottomLeftCorner) },
    { StandardCursorType.TopSide, new Cursor(StandardCursorType.TopSide) },
    { StandardCursorType.BottomSide, new Cursor(StandardCursorType.BottomSide) },
    { StandardCursorType.TopRightCorner, new Cursor(StandardCursorType.TopRightCorner) },
    { StandardCursorType.RightSide, new Cursor(StandardCursorType.RightSide) },
    { StandardCursorType.BottomRightCorner, new Cursor(StandardCursorType.BottomRightCorner) }
  };

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

  private Cursor GetCursorForDirection((HorizontalAlignment Horizontal, VerticalAlignment Vertical) direction)
  {
    if (direction.Horizontal == HorizontalAlignment.Center && direction.Vertical == VerticalAlignment.Center)
    {
      return Cursor.Default;
    }

    if (direction.Horizontal == HorizontalAlignment.Stretch || direction.Vertical == VerticalAlignment.Stretch)
    {
      throw new InvalidOperationException("Stretch unexpected.");
    }

    return direction.Horizontal switch
    {
      HorizontalAlignment.Left => direction.Vertical == VerticalAlignment.Top
        ? cursors[StandardCursorType.TopLeftCorner]
        : direction.Vertical == VerticalAlignment.Center
          ? cursors[StandardCursorType.LeftSide]
          : cursors[StandardCursorType.BottomLeftCorner],
      HorizontalAlignment.Center => direction.Vertical == VerticalAlignment.Top
        ? cursors[StandardCursorType.TopSide]
        : cursors[StandardCursorType.BottomSide],
      HorizontalAlignment.Right => direction.Vertical == VerticalAlignment.Top
        ? cursors[StandardCursorType.TopRightCorner]
        : direction.Vertical == VerticalAlignment.Center
          ? cursors[StandardCursorType.RightSide]
          : cursors[StandardCursorType.BottomRightCorner],
      _ => throw new InvalidOperationException($"{direction.Horizontal}:{direction.Vertical} unexpected."),
    };
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
      else if (cursors.Any(c => ReferenceEquals(c.Value, targetControl.Cursor)))
      {
        targetControl.Cursor = Cursor.Default;
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

    targetControl.Cursor = Cursor.Default;
    isResizing = false;
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

    if (resizeDirection.Horizontal == HorizontalAlignment.Left)
    {
      viewModel.X = x - targetOffset.X;
      viewModel.Width += targetOffset.X;
    }
    else if (resizeDirection.Horizontal == HorizontalAlignment.Right)
    {
      viewModel.Width -= targetOffset.X;
    }

    if (resizeDirection.Vertical == VerticalAlignment.Top)
    {
      viewModel.Y = y - targetOffset.Y;
      viewModel.Height += targetOffset.Y;
    }
    else if (resizeDirection.Vertical == VerticalAlignment.Bottom)
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

      return !(resizeDirection.Horizontal == HorizontalAlignment.Center
        && resizeDirection.Vertical == VerticalAlignment.Center);
    }

    return false;
  }

  private bool CanResize(PointerEventArgs e) => e.GetCurrentPoint(targetControl).Properties.IsLeftButtonPressed;

  private (HorizontalAlignment, VerticalAlignment) GetDirection(Point point)
  {
    var cornerLength = Math.Min(targetControl.Bounds.Height, targetControl.Bounds.Width) / 3d;

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
}