using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using System;

namespace ControlsSandbox.Behaviors;

public sealed class DraggableControlBehavior : ControlBehavior
{
  private readonly UserControl targetControl;

  private PixelPoint targetPosition;
  private Point lastMousePosition;
  private readonly DispatcherTimer timer;
  private bool isDragging;
  private IControlBounds viewModel;

  public DraggableControlBehavior(UserControl userControl)
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
    if (!CanStartDrag(e))
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
    targetPosition = new PixelPoint((int)viewModel.X, (int)viewModel.Y);

    isDragging = true;
    targetControl.Cursor = new Cursor(StandardCursorType.SizeAll);
    e.Handled = true;

    timer.Start();
  }

  private void OnPointerMoved(object sender, PointerEventArgs e)
  {
    if (!(isDragging && CanDrag(e)))
    {
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

    timer.Stop();
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