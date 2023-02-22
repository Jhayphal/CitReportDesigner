namespace CitReport;

public abstract class Block
{
  public string Code { get; set; }

  public readonly List<Option> Options = new();
}