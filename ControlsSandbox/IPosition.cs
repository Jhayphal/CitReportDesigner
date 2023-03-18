namespace ControlsSandbox;

public interface IPosition
{
  double X { get; set; }

  double Y { get; set; }

  ReportSizeUnit SizeUnit { get; }
}