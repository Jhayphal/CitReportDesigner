using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System;

namespace ControlsSandbox.Behaviors;

public sealed class DraggableControlBehavior<TRelativeTo> : ControlBehavior
  where TRelativeTo : class, IVisual
{
  private readonly UserControl targetControl;
  private readonly DispatcherTimer applyPositionTimer;
  private readonly Cursor moveCursor = new(StandardCursorType.SizeAll);
  private readonly KeyModifiers activateWithModifiers;

  private IControlBounds viewModel;
  private Point targetPosition;
  private Point lastMousePosition;
  private bool isDragging;
  private TRelativeTo relativeTo;

  private volatile bool isProgress;

  public DraggableControlBehavior(UserControl userControl, KeyModifiers activateWith)
  {
    targetControl = userControl;
    activateWithModifiers = activateWith;

    targetControl.PointerPressed += TryStartDrag;
    targetControl.PointerMoved += GetPosition;

    applyPositionTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
    applyPositionTimer.Tick += ApplyPosition;

    targetControl.PointerReleased += RestoreCursor;
  }

  private void TryStartDrag(object sender, PointerPressedEventArgs e)
  {
    if (!CanStartDrag(e))
    {
      return;
    }

    viewModel = targetControl.DataContext as IControlBounds;
    if (viewModel == null)
    {
      return;
    }

    relativeTo = targetControl.FindAncestorOfType<TRelativeTo>();
    if (relativeTo == null)
    {
      return;
    }

    if (viewModel.SizeUnit == ReportSizeUnit.Millimeter)
    {
      viewModel = new MillimetersToPixelsBoundsAdapter(viewModel);
    }

    lastMousePosition = e.GetPosition(targetControl);
    targetPosition = new Point(viewModel.X, viewModel.Y);

    isDragging = true;
    targetControl.Cursor = moveCursor;
    e.Handled = true;

    applyPositionTimer.Start();
  }

  private bool CanStartDrag(PointerEventArgs e) => e.KeyModifiers == activateWithModifiers && CanDrag(e);

  private bool CanDrag(PointerEventArgs e) => e.GetCurrentPoint(targetControl).Properties.IsLeftButtonPressed;

  private void GetPosition(object sender, PointerEventArgs e)
  {
    if (isDragging && CanDrag(e))
    {
      var currentMousePosition = e.GetPosition(targetControl);
      var offset = currentMousePosition - lastMousePosition;

      targetPosition = new Point(viewModel.X + offset.X, viewModel.Y + offset.Y);
    }
    else if (e.KeyModifiers == KeyModifiers.Control)
    {
      targetControl.Cursor = moveCursor;
    }
    else if (ReferenceEquals(targetControl.Cursor, moveCursor))
    {
      targetControl.Cursor = Cursor.Default;
    }
  }

  private void ApplyPosition(object sender, EventArgs e)
  {
    if (isProgress
      || (int)viewModel.X == targetPosition.X
      && (int)viewModel.Y == targetPosition.Y)
    {
      return;
    }

    isProgress = true;

    if (double.IsNegative(targetPosition.X))
    {
      viewModel.X = 0d;
    }
    else if (double.IsNegative(relativeTo.Bounds.Width - (targetPosition.X + viewModel.Width)))
    {
      viewModel.X = relativeTo.Bounds.Width - viewModel.Width;
    }
    else
    {
      viewModel.X = targetPosition.X;
    }

    if (double.IsNegative(targetPosition.Y))
    {
      viewModel.Y = 0d;
    }
    else if (double.IsNegative(relativeTo.Bounds.Height - (targetPosition.Y + viewModel.Height)))
    {
      viewModel.Y = relativeTo.Bounds.Height - viewModel.Height;
    }
    else
    {
      viewModel.Y = targetPosition.Y;
    }

    isProgress = false;
  }

  private void RestoreCursor(object sender, PointerReleasedEventArgs e)
  {
    if (!isDragging)
    {
      return;
    }

    isDragging = false;
    targetControl.Cursor = Cursor.Default;
    e.Handled = true;

    applyPositionTimer.Stop();
  }
}