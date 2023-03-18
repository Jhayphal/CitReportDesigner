namespace ControlsSandbox;

public interface ISize : IMeasurable
{
  double Width { get; set; }

  double Height { get; set; }
}