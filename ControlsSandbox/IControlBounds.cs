namespace ControlsSandbox;

public interface IControlBounds
{
  double X { get; set; }

  double Y { get; set; }

  double Width { get; set; }

  double Height { get; set; }

  ReportSizeUnit SizeUnit { get; }
}