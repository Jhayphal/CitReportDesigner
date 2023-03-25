using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace ControlsSandbox.Controls;

public class LineControl : Control
{
  private Pen pen;
  
  static LineControl()
  {
    AffectsRender<LineControl>(X1Property);
    AffectsRender<LineControl>(Y1Property);
    AffectsRender<LineControl>(X2Property);
    AffectsRender<LineControl>(Y2Property);
    AffectsRender<LineControl>(ColorProperty);
    AffectsRender<LineControl>(ThicknessProperty);
  }

  public LineControl()
  {
    Color = Brushes.Black;
    Thickness = 1;
    HorizontalAlignment = HorizontalAlignment.Left;
    VerticalAlignment = VerticalAlignment.Top;
  }

  public static readonly StyledProperty<double> X1Property = AvaloniaProperty.Register<LineControl, double>(nameof(X1));
  public static readonly StyledProperty<double> Y1Property = AvaloniaProperty.Register<LineControl, double>(nameof(Y1));
  public static readonly StyledProperty<double> X2Property = AvaloniaProperty.Register<LineControl, double>(nameof(X2));
  public static readonly StyledProperty<double> Y2Property = AvaloniaProperty.Register<LineControl, double>(nameof(Y2));
  public static readonly StyledProperty<IBrush> ColorProperty = AvaloniaProperty.Register<LineControl, IBrush>(nameof(Color));
  public static readonly StyledProperty<double> ThicknessProperty = AvaloniaProperty.Register<LineControl, double>(nameof(Thickness));

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
      Width = Math.Abs(X2 - X1);
    }
  }

  public double Y2
  {
    get => GetValue(Y2Property);
    set
    {
      SetValue(Y2Property, value);
      Height = Math.Abs(Y2 - Y1);
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

  public override void Render(DrawingContext context)
  {
    if (!ReferenceEquals(pen?.Brush, Color))
    {
      pen = new Pen(Color, Thickness);
    }
    
    context.DrawLine(pen, new Point(X1, Y1), new Point(X2, Y2));
  }
}