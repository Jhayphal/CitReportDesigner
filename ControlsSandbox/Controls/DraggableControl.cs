using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using System;

namespace ControlsSandbox.Controls
{
  public abstract class DraggableControl : UserControl
  {
    /// <summary>
    /// Record the last mouse location
    /// </summary>
    private Point lastMousePosition;

    /// <summary>
    /// The timer for smooth updating coordinates
    /// </summary>
    private DispatcherTimer timer;

    /// <summary>
    /// Whether the marker starts first and drags
    /// </summary>
    private bool isDragging = false;

    /// <summary>
    /// Need the coordinate point that needs to be updated
    /// </summary>
    private PixelPoint targetPosition;

    public DraggableControl()
    {
      InitializeComponent();

      // Add the current control event to monitor
      PointerPressed += OnPointerPressed;
      PointerMoved += OnPointerMoved;
      PointerReleased += OnPointerReleased;

      // Initialize the timer
      timer = new DispatcherTimer
      {
        Interval = TimeSpan.FromMilliseconds(10)
      };
      timer.Tick += OnTimerTick;
    }

    /// <summary>
    /// Times event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnTimerTick(object sender, EventArgs e)
    {
      var window = this.FindAncestorOfType<Window>();
      if (window != null && window.Position != targetPosition)
      {
        // Update coordinates
        window.Position = targetPosition;
      }
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }

    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
      if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
      {
        return;
      }

      // Start Drag
      isDragging = true;
      
      // Record the current coordinates
      lastMousePosition = e.GetPosition(this);
      e.Handled = true;
      
      // Start the timer
      timer.Start();
    }

    private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
    {
      if (!isDragging)
      {
        return;
      }

      // Stop dragging
      isDragging = false;
      e.Handled = true;
      
      // Stop the timer
      timer.Stop();
    }

    private void OnPointerMoved(object sender, PointerEventArgs e)
    {
      if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
      {
        return;
      }

      // If the drag is not started, it will not be executed
      if (!isDragging)
      {
        return;
      }

      var currentMousePosition = e.GetPosition(this);
      var offset = currentMousePosition - lastMousePosition;
      
      var window = this.FindAncestorOfType<Window>();
      if (window != null)
      {
        // Record the current coordinates
        targetPosition = new PixelPoint(window.Position.X + (int)offset.X,
            window.Position.Y + (int)offset.Y);
      }
    }
  }
}
