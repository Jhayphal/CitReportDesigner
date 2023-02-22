namespace CitReport.Services.Parser
{
  internal sealed class FontParser : BlockInstructionParser
  {
    private readonly string[] supportedInstructions = new string[] { Instructions.Fl };

    protected override CodeContext ActualContext => CodeContext.Block;

    protected override IEnumerable<string> SupportedInstructions => supportedInstructions;

    /// <summary>
    /// Parse font.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="current"></param>
    /// <remarks>{/FL, FONT1, Times New Roman, 10, B}</remarks>
    public override void Parse(ParserContext context, string current)
    {
      var parts = Tokenizer.GetTokens(current)
        .Select(x => x.Trim())
        .ToList();

      if (parts.Count < 8)
      {
        context.ErrorProvider.AddError($"Unfinished instruction: '{current}'.");
        return;
      }

      var alias = parts[3];

      var font = new FontInfo
      {
        Family = parts[5],
        Size = float.TryParse(parts[7], out var size) ? size : 0
      };

      if (parts.Count > 9)
      {
        font.Style = string.Equals(parts[9], "B", StringComparison.OrdinalIgnoreCase)
          ? FontStyle.Bold
          : FontStyle.Regular;
      }

      context.CurrentBlock.Fonts.Add(alias, font);
    }
  }
}
