namespace ControlsSandbox;

public class DesignerEnvironment
{
  public static readonly DesignerEnvironment Current = new();

  public string Language { get; set; } = string.Empty;
}
