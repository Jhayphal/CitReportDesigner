namespace CitReport.IO.Parser;

public class TableColumnsParser : BlockInstructionParser
{
  protected override CodeContext ActualContext => CodeContext.Block;

  protected override IEnumerable<string> SupportedInstructions { get; } = new string[] { Instructions.Tc };

  public override void Parse(ParserContext context, string current)
  {
    context.Columns = ParseArray(current);
    CreateTableIfRequired(context);
  }

  private static void CreateTableIfRequired(ParserContext context)
  {
    context.SetTableAsCurrent(null);

    if (context.Columns == null || context.Rows == null)
    {
      return;
    }

    if (context.Columns.Length == 0)
    {
      context.ErrorProvider.TableHasNotColumns(context.CurrentLine);
    }
    else if (context.Rows.Length == 0)
    {
      context.ErrorProvider.TableHasNotRows(context.CurrentLine);
    }
    else
    {
      context.SetTableAsCurrent(new Table(context.Columns, context.Rows));
    }

    context.Columns = null;
    context.Rows = null;
  }

  private double[] ParseArray(string current)
    => Tokenizer.GetTokens(current)
      .Where(x => double.TryParse(x, out var _))
      .Select(double.Parse)
      .ToArray();
}
