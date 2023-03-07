namespace CitReport.IO.Parser;

public class BodyBlockParser : IInstructionParser
{
  private readonly OptionsParser optionsParser = new();

  public bool CanParse(string current, CodeContext context)
    => (context == CodeContext.Block || context == CodeContext.ReportDefinition)
      && current.StartsWith(Instructions.Blk, StringComparison.OrdinalIgnoreCase);

  public void Parse(ParserContext context, string current)
  {
    var blockId = (context.Report.Blocks.LastOrDefault()?.Id).GetValueOrDefault() + 1;
    var block = new BodyBlock(context.Report, blockId);

    var parts = current.Split(' ', StringSplitOptions.RemoveEmptyEntries);

    if (parts.Length > 1)
      block.Code = parts[1];

    if (parts.Length > 2)
    {
      var options = string.Join(" ", parts.Skip(2));
      block.Options.AddRange(optionsParser.Parse(context, options));
    }

    context.SetContext(CodeContext.Block);
    context.Report.Blocks.Add(block);
    context.SetBlockAsCurrent(block);
  }
}
