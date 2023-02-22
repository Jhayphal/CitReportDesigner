namespace CitReport;

public class ReportBlock : Block
{
  public readonly List<Expression> AfterStartActions = new();

  public readonly List<Expression> AfterEndActions = new();

  public readonly List<Expression> DoActions = new();
}