namespace CitReport.Services.Parser
{
  internal class BodyBlockParser : IInstructionParser
  {
    private readonly OptionsParser optionsParser = new();

    public bool CanParse(string current, CodeContext context) => current.StartsWith(Instructions.Blk, StringComparison.OrdinalIgnoreCase);

    public void Parse(ParserContext context, string current)
    {
      var block = new BodyBlock();

      var parts = current.Split(' ', StringSplitOptions.RemoveEmptyEntries);

      if (parts.Length > 1)
        block.Code = parts[1];

      if (parts.Length > 2)
      {
        var options = string.Join(" ", parts.Skip(2));
        block.Options.AddRange(optionsParser.Parse(options, context.ErrorProvider));
      }

      context.Report.BodyBlocks.Add(block);
      context.SetBlockAsCurrent(block);
    }
  }
}
