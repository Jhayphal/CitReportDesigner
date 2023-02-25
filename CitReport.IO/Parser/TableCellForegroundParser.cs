using System.Drawing;

namespace CitReport.IO.Parser;

public class TableCellForegroundParser : BlockInstructionParser
{
  protected override CodeContext ActualContext => CodeContext.Block;

  protected override IEnumerable<string> SupportedInstructions { get; } = new string[] { Instructions.Ct };

  public override void Parse(ParserContext context, string current)
  {
    if (context.CurrentTable != null)
    {
      ParseForegroundColor(context, current);
    }
    else
    {
      context.ErrorProvider.UnexpectedTableInstructionLocation(current, context.CurrentLine);
    }
  }

  /// <summary>
  /// Parse text color.
  /// </summary>
  /// <param name="context"></param>
  /// <param name="current"></param>
  /// <remarks>{/CT, 0, 128, 128}</remarks>
  private void ParseForegroundColor(ParserContext context, string current)
  {
    var tokens = Tokenizer.GetTokens(current)
      .Select(x => x.Trim())
      .Where(x => byte.TryParse(x, out _))
      .Select(byte.Parse)
      .ToArray();

    if (tokens.Length != 3)
    {
      context.ErrorProvider.WrongInstructionBody(current, context.CurrentLine);
      return;
    }

    context.CellForegroundColor = Color.FromArgb(tokens[0], tokens[1], tokens[2]);
  }
}