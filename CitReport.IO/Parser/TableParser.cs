using System.Drawing;

namespace CitReport.IO.Parser;

public class TableParser : BlockInstructionParser
{
  private float[] columns;
  private float[] rows;
  private Table table;
  private Color foregroundColor = Color.Black;
  private Color backgroundColor = Color.White;

  private readonly string[] supportedInstructions = new string[] 
  { 
    Instructions.Tc, 
    Instructions.Tr, 
    Instructions.Ts 
  };

  private readonly string[] fieldInstructions = new string[]
  {
    Instructions.Ts,
    Instructions.Tsg
  };

  protected override CodeContext ActualContext => CodeContext.Block;

  protected override IEnumerable<string> SupportedInstructions => supportedInstructions;

  public override void Parse(ParserContext context, string current)
  {
    if (IsInstructionSupported(current, Instructions.Tc))
    {
      columns = ParseArray(current);
      CreateTableIfRequired(context);
    }
    else if (IsInstructionSupported(current, Instructions.Tr))
    {
      rows = ParseArray(current);
      CreateTableIfRequired(context);
    }
    else if (table != null)
    {
      if (IsInstructionSupported(current, fieldInstructions))
      {
        ParseCell(context, current);
      }
      else if (IsInstructionSupported(current, Instructions.Ct))
      {
        ParseForegroundColor(context, current);
      }
    }
    else
    {
      context.ErrorProvider.AddError($"Unexpected table instruction outside table definition.");
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
      context.ErrorProvider.AddError($"Wrong instruction data: '{current}'.");
      return;
    }

    foregroundColor = Color.FromArgb(tokens[0], tokens[1], tokens[2]);
  }

  /// <summary>
  /// Parse table cell.
  /// </summary>
  /// <param name="context"></param>
  /// <param name="current"></param>
  /// <remarks>For example, {/TS, A2:A2, , FONT1}Field ___</remarks>
  private void ParseCell(ParserContext context, string current)
  {
    var tokens = Tokenizer.GetTokens(current)
      .Select(x => x.Trim())
      .ToList();
    var endDefinitionIndex = tokens.IndexOf("}");
    if (endDefinitionIndex < 0 || tokens.Count < 11)
    {
      context.ErrorProvider.AddError($"Unfinished instruction: '{current}'.");
      return;
    }

    var canGrow = string.Equals(tokens[1], "/TSG", StringComparison.OrdinalIgnoreCase);
    var languageSpecifed = tokens[2] == ":";
    var language = languageSpecifed ? tokens[3] : string.Empty;
    var offset = languageSpecifed ? 2 : 0;
    var cellPositionLeftUpper = CellPosition.FromString(tokens[3 + offset].Trim());
    var cellPositionRightBottom = CellPosition.FromString(tokens[5 + offset].Trim());
    Cell cell = table.Merge(cellPositionLeftUpper, cellPositionRightBottom);
    cell.CanGrow = canGrow;
    var displayValueIndex = endDefinitionIndex + 1;
    var displayValue = displayValueIndex >= tokens.Count
      ? string.Empty
      : tokens[displayValueIndex];
    cell.DisplayValue[language] = displayValue;

    if (displayValue.Contains('_'))
    {
      context.Fields.Enqueue(new ValueTarget(cell, language));
    }

    var fontAlias = tokens[9 + offset].Trim();
    if (context.CurrentBlock.Fonts.TryGetValue(fontAlias, out var fontInfo))
    {
      cell.Font = fontInfo;
    }
    else
    {
      context.ErrorProvider.AddError($"Undefined font '{fontAlias}'.");
    }

    cell.ForegroundColor = foregroundColor;
    cell.BackgroundColor = backgroundColor;
  }

  private void CreateTableIfRequired(ParserContext context)
  {
    table = null;

    if (columns == null || rows == null)
    {
      return;
    }

    if (columns.Length == 0)
    {
      context.ErrorProvider.AddError("Table has not columns.");
    }
    else if (rows.Length == 0)
    {
      context.ErrorProvider.AddError("Table has not rows.");
    }
    else
    {
      table = new Table(columns, rows);
      columns = null;
      rows = null;
    }
  }

  private float[] ParseArray(string current)
    => Tokenizer.GetTokens(current)
      .Where(x => float.TryParse(x, out var _))
      .Select(float.Parse)
      .ToArray();
}
