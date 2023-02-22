namespace CitReport.IO.Parser;

public class CodeBehindParser : IInstructionParser
{
  public bool CanParse(string current, CodeContext context) => context == CodeContext.CodeBehind;

  public void Parse(ParserContext context, string current)
  {
    context.Report.CodeBehind.Add(new Expression { Value = current });
  }
}
