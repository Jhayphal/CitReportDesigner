using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using ControlsSandbox.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlsSandbox.Behaviors;

public sealed class ResizableControlBehavior<TRelativeTo> : ControlBehavior
  where TRelativeTo : class, IVisual
{
  private readonly UserControl targetControl;
  private readonly DispatcherTimer applySizeTimer;
  private readonly Dictionary<StandardCursorType, Cursor> sizingCursors = new()
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
  private readonly Size minimumSize;
  private readonly KeyModifiers activateWithModifiers;

  private IBounds viewModel;
  private Point targetOffset;
  private Point lastMousePosition;
  private bool isResizing;
  private ControlBounds originalBounds;
  private (HorizontalAlignment Horizontal, VerticalAlignment Vertical) resizeDirection;
  private TRelativeTo relativeTo;
  private bool findLastAncestor;

  private volatile bool isProgress;

  public ResizableControlBehavior(UserControl userControl, KeyModifiers activateWith, Size minimalSize, bool findLastAncestor = false)
  {
    targetControl = userControl;
    activateWithModifiers = activateWith;
    minimumSize = minimalSize;
    this.findLastAncestor = findLastAncestor;

    targetControl.PointerPressed += TryStartResize;
    targetControl.PointerMoved += GetPointerPosition;
    targetControl.PointerReleased += RestoreCursor;

    applySizeTimer = new DispatcherTimer
    {
      Interval = TimeSpan.FromMilliseconds(10)
    };

    applySizeTimer.Tick += ApplySize;
  }

  private void TryStartResize(object sender, PointerPressedEventArgs e)
  {
    if (!CanStartResize(e))
    {
      return;
    }

    viewModel = targetControl.DataContext as IBounds;
    if (viewModel == null)
    {
      return;
    }

    relativeTo = FindAncestor();
    if (relativeTo == null)
    {
      return;
    }

    if (viewModel.SizeUnit == ReportSizeUnit.Millimeter)
    {
      viewModel = new MillimetersToPixelsBoundsAdapter(viewModel);
    }

    targetOffset = new Point();
    originalBounds = new ControlBounds(viewModel);
    lastMousePosition = e.GetPosition(relativeTo);

    isResizing = true;
    e.Handled = true;

    applySizeTimer.Start();
  }

  private TRelativeTo FindAncestor()
  {
    var previouslyAncestor = targetControl.FindAncestorOfType<TRelativeTo>();
    if (findLastAncestor)
    {
      var currentAncestor = previouslyAncestor.FindAncestorOfType<TRelativeTo>();
      while (currentAncestor != null)
      {
        previouslyAncestor = currentAncestor;
        currentAncestor = previouslyAncestor.FindAncestorOfType<TRelativeTo>();
      }
    }
    return previouslyAncestor;
  }

  private bool CanStartResize(PointerEventArgs e)
  {
    if (e.KeyModifiers != activateWithModifiers || !CanResize(e))
    {
      return false;
    }

    resizeDirection = GetDirection(e.GetPosition(targetControl));

    return !(resizeDirection.Horizontal == HorizontalAlignment.Center
      && resizeDirection.Vertical == VerticalAlignment.Center);
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

  private void GetPointerPosition(object sender, PointerEventArgs e)
  {
    if (isResizing && CanResize(e))
    {
      var currentMousePosition = e.GetPosition(relativeTo);
      targetOffset = lastMousePosition - currentMousePosition;
    }
    else if (e.KeyModifiers == activateWithModifiers)
    {
      targetControl.Cursor = GetCursorForDirection(GetDirection(e.GetPosition(targetControl)));
    }
    else if (sizingCursors.Any(c => ReferenceEquals(c.Value, targetControl.Cursor)))
    {
      targetControl.Cursor = Cursor.Default;
    }
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
        ? sizingCursors[StandardCursorType.TopLeftCorner]
        : direction.Vertical == VerticalAlignment.Center
          ? sizingCursors[StandardCursorType.LeftSide]
          : sizingCursors[StandardCursorType.BottomLeftCorner],
      HorizontalAlignment.Center => direction.Vertical == VerticalAlignment.Top
        ? sizingCursors[StandardCursorType.TopSide]
        : sizingCursors[StandardCursorType.BottomSide],
      HorizontalAlignment.Right => direction.Vertical == VerticalAlignment.Top
        ? sizingCursors[StandardCursorType.TopRightCorner]
        : direction.Vertical == VerticalAlignment.Center
          ? sizingCursors[StandardCursorType.RightSide]
          : sizingCursors[StandardCursorType.BottomRightCorner],
      _ => throw new InvalidOperationException($"{direction.Horizontal}:{direction.Vertical} unexpected."),
    };
  }

  private void ApplySize(object sender, EventArgs e)
  {
    if (isProgress)
    {
      return;
    }

    isProgress = true;

    var newBounds = new ControlBounds(originalBounds);

    if (resizeDirection.Horizontal == HorizontalAlignment.Left)
    {
      newBounds.X = originalBounds.X - targetOffset.X;

      if (double.IsNegative(newBounds.X))
      {
        newBounds.Width += targetOffset.X + newBounds.X;
        newBounds.X = 0d;
      }
      else
      {
        newBounds.Width += targetOffset.X;

        if (newBounds.Width - minimumSize.Width < double.Epsilon)
        {
          newBounds.X += newBounds.Width - minimumSize.Width;
          newBounds.Width = minimumSize.Width;
        }
      }

      viewModel.X = newBounds.X;
      viewModel.Width = newBounds.Width;
    }
    else if (resizeDirection.Horizontal == HorizontalAlignment.Right)
    {
      newBounds.Width -= targetOffset.X;

      if (newBounds.Width - minimumSize.Width > double.Epsilon)
      {
        if (newBounds.X + newBounds.Width > relativeTo.Bounds.Width)
        {
          newBounds.Width = relativeTo.Bounds.Width - newBounds.X;
        }

        viewModel.Width = newBounds.Width;
      }
      else
      {
        viewModel.Width = minimumSize.Width;
      }
    }

    if (resizeDirection.Vertical == VerticalAlignment.Top)
    {
      newBounds.Y = originalBounds.Y - targetOffset.Y;

      if (double.IsNegative(newBounds.Y))
      {
        newBounds.Height += targetOffset.Y + newBounds.Y;
        newBounds.Y = 0d;
      }
      else
      {
        newBounds.Height += targetOffset.Y;

        if (newBounds.Height - minimumSize.Height < double.Epsilon)
        {
          newBounds.Y += newBounds.Height - minimumSize.Height;
          newBounds.Height = minimumSize.Height;
        }
      }

      viewModel.Y = newBounds.Y;
      viewModel.Height = newBounds.Height;
    }
    else if (resizeDirection.Vertical == VerticalAlignment.Bottom)
    {
      newBounds.Height -= targetOffset.Y;

      if (newBounds.Height - minimumSize.Height > double.Epsilon)
      {
        if (newBounds.Y + newBounds.Height > relativeTo.Bounds.Height)
        {
          newBounds.Height = relativeTo.Bounds.Height - newBounds.Y;
        }

        viewModel.Height = newBounds.Height;
      }
      else
      {
        viewModel.Height = minimumSize.Height;
      }
    }

    isProgress = false;
  }

  private void RestoreCursor(object sender, PointerReleasedEventArgs e)
  {
    if (!isResizing)
    {
      return;
    }

    targetControl.Cursor = Cursor.Default;
    isResizing = false;
    e.Handled = true;

    applySizeTimer.Stop();
  }
}

public sealed class ResizableCellBehavior<TRelativeTo> : ControlBehavior
  where TRelativeTo : class, IVisual
{
  private readonly UserControl targetControl;
  private readonly DispatcherTimer applySizeTimer;
  private readonly Dictionary<StandardCursorType, Cursor> sizingCursors = new()
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
  private readonly Size minimumSize;
  private readonly KeyModifiers activateWithModifiers;

  private IBounds viewModel;
  private CellViewModel cell;
  private Point targetOffset;
  private Point lastMousePosition;
  private bool isResizing;
  private ControlBounds originalBounds;
  private (HorizontalAlignment Horizontal, VerticalAlignment Vertical) resizeDirection;
  private TRelativeTo relativeTo;
  private bool findLastAncestor;

  private volatile bool isProgress;

  public ResizableCellBehavior(UserControl userControl, KeyModifiers activateWith, Size minimalSize, bool findLastAncestor = false)
  {
    targetControl = userControl;
    activateWithModifiers = activateWith;
    minimumSize = minimalSize;
    this.findLastAncestor = findLastAncestor;

    targetControl.PointerPressed += TryStartResize;
    targetControl.PointerMoved += GetPointerPosition;
    targetControl.PointerReleased += RestoreCursor;

    applySizeTimer = new DispatcherTimer
    {
      Interval = TimeSpan.FromMilliseconds(10)
    };

    applySizeTimer.Tick += ApplySize;
  }

  private void TryStartResize(object sender, PointerPressedEventArgs e)
  {
    if (!CanStartResize(e))
    {
      return;
    }

    viewModel = targetControl.DataContext as IBounds;
    if (viewModel == null)
    {
      return;
    }

    cell = viewModel as CellViewModel;
    if (cell == null)
    {
      return;
    }

    relativeTo = FindAncestor();
    if (relativeTo == null)
    {
      return;
    }

    if (viewModel.SizeUnit == ReportSizeUnit.Millimeter)
    {
      viewModel = new MillimetersToPixelsBoundsAdapter(viewModel);
    }

    targetOffset = new Point();
    originalBounds = new ControlBounds(viewModel);
    lastMousePosition = e.GetPosition(relativeTo);

    isResizing = true;
    e.Handled = true;

    applySizeTimer.Start();
  }

  private TRelativeTo FindAncestor()
  {
    var previouslyAncestor = targetControl.FindAncestorOfType<TRelativeTo>();
    if (findLastAncestor)
    {
      var currentAncestor = previouslyAncestor.FindAncestorOfType<TRelativeTo>();
      while (currentAncestor != null)
      {
        previouslyAncestor = currentAncestor;
        currentAncestor = previouslyAncestor.FindAncestorOfType<TRelativeTo>();
      }
    }
    return previouslyAncestor;
  }

  private bool CanStartResize(PointerEventArgs e)
  {
    if (e.KeyModifiers != activateWithModifiers || !CanResize(e))
    {
      return false;
    }

    resizeDirection = GetDirection(e.GetPosition(targetControl));

    return !(resizeDirection.Horizontal == HorizontalAlignment.Center
      && resizeDirection.Vertical == VerticalAlignment.Center);
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

  private void GetPointerPosition(object sender, PointerEventArgs e)
  {
    if (isResizing && CanResize(e))
    {
      var currentMousePosition = e.GetPosition(relativeTo);
      targetOffset = lastMousePosition - currentMousePosition;
    }
    else if (e.KeyModifiers == activateWithModifiers)
    {
      targetControl.Cursor = GetCursorForDirection(GetDirection(e.GetPosition(targetControl)));
    }
    else if (sizingCursors.Any(c => ReferenceEquals(c.Value, targetControl.Cursor)))
    {
      targetControl.Cursor = Cursor.Default;
    }
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
        ? sizingCursors[StandardCursorType.TopLeftCorner]
        : direction.Vertical == VerticalAlignment.Center
          ? sizingCursors[StandardCursorType.LeftSide]
          : sizingCursors[StandardCursorType.BottomLeftCorner],
      HorizontalAlignment.Center => direction.Vertical == VerticalAlignment.Top
        ? sizingCursors[StandardCursorType.TopSide]
        : sizingCursors[StandardCursorType.BottomSide],
      HorizontalAlignment.Right => direction.Vertical == VerticalAlignment.Top
        ? sizingCursors[StandardCursorType.TopRightCorner]
        : direction.Vertical == VerticalAlignment.Center
          ? sizingCursors[StandardCursorType.RightSide]
          : sizingCursors[StandardCursorType.BottomRightCorner],
      _ => throw new InvalidOperationException($"{direction.Horizontal}:{direction.Vertical} unexpected."),
    };
  }

  private void ApplySize(object sender, EventArgs e)
  {
    if (isProgress)
    {
      return;
    }

    isProgress = true;

    var newBounds = new ControlBounds(originalBounds);

    if (resizeDirection.Horizontal == HorizontalAlignment.Left)
    {
      newBounds.X = originalBounds.X - targetOffset.X;

      if (double.IsNegative(newBounds.X))
      {
        newBounds.Width += targetOffset.X + newBounds.X;
        newBounds.X = 0d;
      }
      else
      {
        newBounds.Width += targetOffset.X;

        if (newBounds.Width - minimumSize.Width < double.Epsilon)
        {
          newBounds.X += newBounds.Width - minimumSize.Width;
          newBounds.Width = minimumSize.Width;
        }
      }

      viewModel.X = newBounds.X;

      if (cell.Column > 0)
      {
        viewModel.Width = newBounds.Width;
      }
    }
    else if (resizeDirection.Horizontal == HorizontalAlignment.Right)
    {
      newBounds.Width -= targetOffset.X;

      if (newBounds.Width - minimumSize.Width > double.Epsilon)
      {
        if (newBounds.X + newBounds.Width > relativeTo.Bounds.Width)
        {
          newBounds.Width = relativeTo.Bounds.Width - newBounds.X;
        }

        viewModel.Width = newBounds.Width;
      }
      else
      {
        viewModel.Width = minimumSize.Width;
      }
    }

    if (resizeDirection.Vertical == VerticalAlignment.Top)
    {
      newBounds.Y = originalBounds.Y - targetOffset.Y;

      if (double.IsNegative(newBounds.Y))
      {
        newBounds.Height += targetOffset.Y + newBounds.Y;
        newBounds.Y = 0d;
      }
      else
      {
        newBounds.Height += targetOffset.Y;

        if (newBounds.Height - minimumSize.Height < double.Epsilon)
        {
          newBounds.Y += newBounds.Height - minimumSize.Height;
          newBounds.Height = minimumSize.Height;
        }
      }

      viewModel.Y = newBounds.Y;
      
      if (cell.Row > 0)
      {
        viewModel.Height = newBounds.Height;
      }
    }
    else if (resizeDirection.Vertical == VerticalAlignment.Bottom)
    {
      newBounds.Height -= targetOffset.Y;

      if (newBounds.Height - minimumSize.Height > double.Epsilon)
      {
        if (newBounds.Y + newBounds.Height > relativeTo.Bounds.Height)
        {
          newBounds.Height = relativeTo.Bounds.Height - newBounds.Y;
        }

        viewModel.Height = newBounds.Height;
      }
      else
      {
        viewModel.Height = minimumSize.Height;
      }
    }

    isProgress = false;
  }

  private void RestoreCursor(object sender, PointerReleasedEventArgs e)
  {
    if (!isResizing)
    {
      return;
    }

    targetControl.Cursor = Cursor.Default;
    isResizing = false;
    e.Handled = true;

    applySizeTimer.Stop();
  }
}