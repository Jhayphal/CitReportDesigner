namespace CitReport.IO.Parser;

public class AfterStartParser : IInstructionParser
{
  public bool CanParse(string current, CodeContext context)
    => context == CodeContext.ReportDefinition
      && current.StartsWith(Instructions.AfterStart, StringComparison.OrdinalIgnoreCase);

  public void Parse(ParserContext context, string current)
  {
    context.Report.Definition.AfterStartActions.Add(new Expression
    {
      Value = current[Instructions.AfterStart.Length..].TrimStart()
    });
  }
}
