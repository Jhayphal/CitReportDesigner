namespace CitReport.IO;

internal sealed class ReportParser
{
  private readonly IEnumerable<IInstructionParser> parsers = new List<IInstructionParser>
  {
    new MetadataParser(),
    new ReportDefinitionParser(),
    new BodyBlockParser(),
    new FontParser(),
    new TableParser(),
    new FieldParser(),
    new CodeBehindParser()
  };

  private ParserContext context;

  public Report Parse(StreamReader reader, IErrorProvider errorProvider)
  {
    context = new ParserContext(errorProvider);
    
    string current;
    while ((current = reader.ReadLine()) != null)
    {
      Parse(current.TrimStart());
    }

    return context.Report;
  }

  private void Parse(string current)
  {
    foreach (var parser in parsers)
    {
      if (parser.CanParse(current, context.Context))
      {
        parser.Parse(context, current);
        break;
      }
    }
  }
}
