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

  private IControlBounds viewModel;
  private PixelPoint targetPosition;
  private Point lastMousePosition;
  private bool isDragging;
  private TRelativeTo relativeTo;

  public DraggableControlBehavior(UserControl userControl, KeyModifiers activateWith)
  {
    targetControl = userControl;
    ActivateWithModifiers = activateWith;

    targetControl.PointerPressed += OnPointerPressed;
    targetControl.PointerMoved += OnPointerMoved;
    targetControl.PointerReleased += OnPointerReleased;

    applyPositionTimer = new DispatcherTimer
    {
      Interval = TimeSpan.FromMilliseconds(10)
    };

    applyPositionTimer.Tick += OnTimerTick;
  }

  public KeyModifiers ActivateWithModifiers { get; }

  private void OnPointerPressed(object sender, PointerPressedEventArgs e)
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
    targetPosition = new PixelPoint((int)viewModel.X, (int)viewModel.Y);

    isDragging = true;
    targetControl.Cursor = moveCursor;
    e.Handled = true;

    applyPositionTimer.Start();
  }

  private void OnPointerMoved(object sender, PointerEventArgs e)
  {
    if (!(isDragging && CanDrag(e)))
    {
      if (e.KeyModifiers == KeyModifiers.Control)
      {
        targetControl.Cursor = moveCursor;
      }
      else if (ReferenceEquals(targetControl.Cursor, moveCursor))
      {
        targetControl.Cursor = Cursor.Default;
      }

      return;
    }

    var currentMousePosition = e.GetPosition(targetControl);
    var offset = currentMousePosition - lastMousePosition;

    targetPosition = new PixelPoint((int)(viewModel.X + offset.X), (int)(viewModel.Y + offset.Y));
  }

  private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
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

  private void OnTimerTick(object sender, EventArgs e)
  {
    if ((int)viewModel.X == targetPosition.X
      && (int)viewModel.Y == targetPosition.Y)
    {
      return;
    }

    viewModel.X = targetPosition.X < 0d ? 0d : targetPosition.X;
    viewModel.Y = targetPosition.Y < 0d ? 0d : targetPosition.Y;
  }

  private bool CanStartDrag(PointerEventArgs e) => e.KeyModifiers == KeyModifiers.Control && CanDrag(e);

  private bool CanDrag(PointerEventArgs e) => e.GetCurrentPoint(targetControl).Properties.IsLeftButtonPressed;
}