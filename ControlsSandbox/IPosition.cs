namespace ControlsSandbox;

public interface IPosition : IMeasurable
{
  double X { get; set; }

  double Y { get; set; }
}