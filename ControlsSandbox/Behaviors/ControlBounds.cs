namespace ControlsSandbox.Behaviors;

public struct ControlBounds : IControlBounds
{
  public ControlBounds(IControlBounds bounds)
  {
    X = bounds.X;
    Y = bounds.Y;
    Width = bounds.Width;
    Height = bounds.Height;
    SizeUnit = bounds.SizeUnit;
  }

  public double X { get; set; }
  
  public double Y { get; set; }
  
  public double Width { get; set; }

  public double Height { get; set; }

  public ReportSizeUnit SizeUnit { get; set; }
}
