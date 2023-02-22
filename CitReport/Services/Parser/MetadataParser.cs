namespace CitReport.Services.Parser
{
  internal class MetadataParser : IInstructionParser
  {
    public bool CanParse(string current, CodeContext context) => current.StartsWith("{/*");

    public void Parse(ParserContext context, string current)
    {
      var metadata = context.CurrentBlock == null
        ? context.Report.Metadata
        : context.CurrentBlock.Metadata;

      metadata.Add(new Metadata { Value = current[3..^3] });
    }
  }
}
