namespace CitReport.IO.Parser;

public class ReportDefinitionParser : IInstructionParser
{
  public bool CanParse(string current, CodeContext context)
    => (context == CodeContext.CodeBehind || context == CodeContext.ReportDefinition)
      && (current.StartsWith(Instructions.Report, StringComparison.OrdinalIgnoreCase)
        || current.StartsWith(Instructions.AfterStart, StringComparison.OrdinalIgnoreCase)
        || current.StartsWith(Instructions.AfterEnd, StringComparison.OrdinalIgnoreCase)
        || current.StartsWith(Instructions.Do, StringComparison.OrdinalIgnoreCase));

  public void Parse(ParserContext context, string current)
  {
    if (current.StartsWith(Instructions.Report, StringComparison.OrdinalIgnoreCase))
    {
      var parts = current.Split(' ', StringSplitOptions.RemoveEmptyEntries);

      if (parts.Length > 1)
      {
        context.Report.ReportDefinition.Code = parts[1];
      }

      if (parts.Length > 2)
      {
        var options = string.Join(" ", parts.Skip(2));
        var optionsParser = new OptionsParser();
        context.Report.ReportDefinition.Options.AddRange(optionsParser.Parse(options, context.ErrorProvider));
      }
    }
    else
    {
      var _ = TryParse(current, Instructions.AfterStart, context.Report.ReportDefinition.AfterStartActions)
        || TryParse(current, Instructions.AfterEnd, context.Report.ReportDefinition.AfterEndActions)
        || TryParse(current, Instructions.Do, context.Report.ReportDefinition.DoActions);
    }
  }

  private static bool TryParse(string current, string instruction, List<Expression> destination)
  {
    if (current.StartsWith(instruction, StringComparison.OrdinalIgnoreCase))
    {
      destination.Add(new Expression
      {
        Value = current[Instructions.AfterStart.Length..].TrimStart()
      });

      return true;
    }

    return false;
  }
}
