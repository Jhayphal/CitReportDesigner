namespace CitReport.IO.Parser;

public class DoParser : IInstructionParser
{
  public bool CanParse(string current, CodeContext context)
    => context == CodeContext.ReportDefinition
      && current.StartsWith(Instructions.Do, StringComparison.OrdinalIgnoreCase);

  public void Parse(ParserContext context, string current)
  {
    context.Report.Definition.DoActions.Add(new Expression
    {
      Value = current[Instructions.Do.Length..].TrimStart()
    });
  }
}