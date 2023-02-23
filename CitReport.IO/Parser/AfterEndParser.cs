namespace CitReport.IO.Parser;

public class AfterEndParser : IInstructionParser
{
  public bool CanParse(string current, CodeContext context)
    => context == CodeContext.ReportDefinition
      && current.StartsWith(Instructions.AfterEnd, StringComparison.OrdinalIgnoreCase);

  public void Parse(ParserContext context, string current)
  {
    context.Report.Definition.AfterEndActions.Add(new Expression
    {
      Value = current[Instructions.AfterEnd.Length..].TrimStart()
    });
  }
}
