namespace CitReport.IO.Parser;

public class TableCellParser : BlockInstructionParser
{
  protected override CodeContext ActualContext => CodeContext.Block;

  protected override IEnumerable<string> SupportedInstructions { get; } = new string[]
  {
    Instructions.Ts,
    Instructions.Tsg
  };

  public override void Parse(ParserContext context, string current)
  {
    if (context.CurrentTable != null)
    {
      ParseCell(context, current);
    }
    else
    {
      context.ErrorProvider.UnexpectedTableInstructionLocation(current, context.CurrentLine);
    }
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
      context.ErrorProvider.UnfinishedInstruction(current, context.CurrentLine);
      return;
    }

    var canGrow = string.Equals(tokens[1], "/TSG", StringComparison.OrdinalIgnoreCase);
    var languageSpecifed = tokens[2] == ":";
    var language = languageSpecifed ? tokens[3] : string.Empty;
    var offset = languageSpecifed ? 2 : 0;
    var cellPositionLeftUpper = CellPosition.FromString(tokens[3 + offset].Trim());
    var cellPositionRightBottom = CellPosition.FromString(tokens[5 + offset].Trim());
    Cell cell = context.CurrentTable.Merge(cellPositionLeftUpper, cellPositionRightBottom);
    cell.Properties.CanGrow = canGrow;
    var displayValueIndex = endDefinitionIndex + 1;
    var displayValue = displayValueIndex >= tokens.Count
      ? string.Empty
      : tokens[displayValueIndex];
    cell.Properties.DisplayValue[language] = displayValue;
    
    var fieldsCount = DisplayFieldsHelper.GetCount(displayValue);
    if (fieldsCount > 0)
    {
      while (fieldsCount-- > 0)
      {
        context.Fields.Enqueue(new ValueTarget(cell, language));
      }
    }

    var fontAlias = tokens[9 + offset].Trim();
    if (context.CurrentBlock.Fonts.TryGetValue(fontAlias, out var fontInfo))
    {
      cell.Style.Font = fontInfo;
    }
    else
    {
      context.ErrorProvider.UndefinedFont(fontAlias, context.CurrentLine);
    }

    cell.Style.ForegroundColor = context.CellForegroundColor;
    cell.Style.BackgroundColor = context.CellBackgroundColor;
  }
}
