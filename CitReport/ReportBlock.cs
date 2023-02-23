using CitReport.Extensions;

namespace CitReport;

public class ReportBlock : Block
{
  public readonly List<Expression> AfterStartActions = new();

  public readonly List<Expression> AfterEndActions = new();

  public readonly List<Expression> DoActions = new();

  public override bool Equals(Block other)
  {
    var result = base.Equals(other);
    if (result)
    {
      return true;
    }

    if (other is ReportBlock block)
    {
      result = AfterStartActions.AreEquals(block.AfterStartActions);

      if (result)
      {
        return true;
      }

      result = AfterEndActions.AreEquals(block.AfterEndActions);

      if (result)
      {
        return true;
      }

      result = DoActions.AreEquals(block.DoActions);
    }

    return result;
  }
}