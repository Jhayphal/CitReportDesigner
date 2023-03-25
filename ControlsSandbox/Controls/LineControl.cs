using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

namespace ControlsSandbox.Controls;

public class LineControl : Control
{
  private readonly Cursor hoverPointCursor = new(StandardCursorType.Cross);
  private readonly Cursor defaultCursor = Cursor.Default;
  
  private Pen pen;
  private bool inMoveA;
  private bool inMoveB;
  
  static LineControl()
  {
    AffectsRender<LineControl>(X1Property);
    AffectsRender<LineControl>(Y1Property);
    AffectsRender<LineControl>(X2Property);
    AffectsRender<LineControl>(Y2Property);
    AffectsRender<LineControl>(ColorProperty);
    AffectsRender<LineControl>(ThicknessProperty);
    AffectsRender<LineControl>(SelectedProperty);
  }

  public LineControl()
  {
    Color = Brushes.Black;
    Thickness = 1;
    HorizontalAlignment = HorizontalAlignment.Left;
    VerticalAlignment = VerticalAlignment.Top;
    
    Tapped += (sender, e) => Selected = true;
    PointerMoved += OnPointerMoved;
    PointerPressed += OnPointerPressed;
    PointerReleased += OnPointerReleased;
  }

  private void OnPointerReleased(object sender, PointerReleasedEventArgs e)
  {
    inMoveA = inMoveB = false;
    Cursor = defaultCursor;
  }

  private void OnPointerPressed(object sender, PointerPressedEventArgs e)
  {
    var point = e.GetPosition(this);
    inMoveA = ARect.Contains(point);
    inMoveB = BRect.Contains(point);
  }

  private void OnPointerMoved(object sender, PointerEventArgs e)
  {
    if (!Selected)
    {
      return;
    }

    var point = e.GetPosition(this);
    if (inMoveA)
    {
      X1 = point.X;
      Y1 = point.Y;
    }
    else if (inMoveB)
    {
      X2 = point.X;
      Y2 = point.Y;
    }
    else
    {
      Cursor = ARect.Contains(point) || BRect.Contains(point)
        ? hoverPointCursor 
        : defaultCursor;
    }
  }

  public static readonly StyledProperty<double> X1Property = AvaloniaProperty.Register<LineControl, double>(nameof(X1));
  public static readonly StyledProperty<double> Y1Property = AvaloniaProperty.Register<LineControl, double>(nameof(Y1));
  public static readonly StyledProperty<double> X2Property = AvaloniaProperty.Register<LineControl, double>(nameof(X2));
  public static readonly StyledProperty<double> Y2Property = AvaloniaProperty.Register<LineControl, double>(nameof(Y2));
  public static readonly StyledProperty<IBrush> ColorProperty = AvaloniaProperty.Register<LineControl, IBrush>(nameof(Color));
  public static readonly StyledProperty<double> ThicknessProperty = AvaloniaProperty.Register<LineControl, double>(nameof(Thickness));
  public static readonly StyledProperty<bool> SelectedProperty = AvaloniaProperty.Register<LineControl, bool>(nameof(Selected));

  public double X1
  {
    get => GetValue(X1Property);
    set => SetValue(X1Property, value);
  }
  
  public double Y1
  {
    get => GetValue(Y1Property);
    set => SetValue(Y1Property, value);
  }
  
  public double X2
  {
    get => GetValue(X2Property);
    set
    {
      SetValue(X2Property, value);
      Width = Math.Max(X2, X1);
    }
  }

  public double Y2
  {
    get => GetValue(Y2Property);
    set
    {
      SetValue(Y2Property, value);
      Height = Math.Max(Y2, Y1);
    }
  }

  public IBrush Color
  {
    get => GetValue(ColorProperty);
    set => SetValue(ColorProperty, value);
  }

  public double Thickness
  {
    get => GetValue(ThicknessProperty);
    set => SetValue(ThicknessProperty, value);
  }

  public bool Selected
  {
    get => GetValue(SelectedProperty);
    set => SetValue(SelectedProperty, value);
  }

  private Rect ARect => new(X1 - Thickness, Y1 - Thickness, Thickness + Thickness, Thickness + Thickness);

  private Rect BRect => new(X2 - Thickness, Y2 - Thickness, Thickness + Thickness, Thickness + Thickness);
  
  public override void Render(DrawingContext context)
  {
    if (!ReferenceEquals(pen?.Brush, Color))
    {
      pen = new Pen(Color, Thickness);
    }
    
    context.DrawLine(pen, new Point(X1, Y1), new Point(X2, Y2));

    if (Selected)
    {
      var size = Thickness * 2D;
      context.DrawRectangle(Brushes.Blue, null, ARect);
      context.DrawRectangle(Brushes.Red, null, BRect);
    }
  }
}