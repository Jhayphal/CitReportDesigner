namespace CitReport.IO.Parser;

public sealed class MetadataParser : IInstructionParser
{
  private const string Begin = "{/*";
  private const string End = "*/}";

  public bool CanParse(string current, CodeContext context)
    => (context == CodeContext.CodeBehind || context == CodeContext.Block)
      && (current?.StartsWith(Begin) ?? false);

  public void Parse(ParserContext context, string current)
  {
    var metadata = context.CurrentBlock == null
      ? context.Report.Metadata
      : context.CurrentBlock.Metadata;

    var end = current.LastIndexOf(End);
    var value = end < 0
      ? current[3..]
      : current[3..end];

    metadata.Add(new Metadata { Value = value });
  }
}
