namespace CitReport.Services.Parser
{
  internal sealed class ReportParser
  {
    private readonly IEnumerable<IInstructionParser> parsers = new List<IInstructionParser>
    {
      new MetadataParser(),
      new ReportDefinitionParser(),

      new CodeBehindParser()
    };

    private ParserContext context;

    public Report Parse(StreamReader reader, IErrorProvider errorProvider)
    {
      context = new ParserContext(errorProvider);
      
      string current;
      while ((current = reader.ReadLine()) != null)
      {
        Parse(current.Trim());
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
        }
      }
    }
  }

  internal class ParserContext
  {
    public ParserContext(IErrorProvider errorProvider)
    {
      ErrorProvider = errorProvider;
      Report = new Report();
      Context = CodeContext.CodeBehind;
    }

    public Report Report { get; }
    
    public CodeContext Context { get; set; }

    public object CurrentItem { get; set; }

    public IErrorProvider ErrorProvider { get; }
  }

  internal enum CodeContext
  {
    CodeBehind,
    ReportDefinition,
    Block
  }

  internal interface IInstructionParser
  {
    bool CanParse(string current, CodeContext context);

    void Parse(ParserContext context, string current);
  }

  internal class MetadataParser : IInstructionParser
  {
    public bool CanParse(string current, CodeContext context) => current.StartsWith("{/*");

    public void Parse(ParserContext context, string current)
    {
      var metadata = context.CurrentItem switch
      {
        Report report => report.Metadata,
        BodyBlock bodyBlock => bodyBlock.Metadata,
        _ => throw new NotSupportedException(context.CurrentItem.GetType().FullName)
      };

      metadata.Add(new Metadata { Value = current[3..^3] });
    }
  }

  internal static class Instructions
  {
    public const string Report = "/REPORT ";
    public const string AfterStart = "/AFTER START ";
    public const string AfterEnd = "/AFTER END ";
    public const string Do = "/DO ";
  }

  internal class ReportDefinitionParser : IInstructionParser
  {
    public bool CanParse(string current, CodeContext context)
      => current.StartsWith(Instructions.Report, StringComparison.OrdinalIgnoreCase)
        || current.StartsWith(Instructions.AfterStart, StringComparison.OrdinalIgnoreCase)
        || current.StartsWith(Instructions.AfterEnd, StringComparison.OrdinalIgnoreCase)
        || current.StartsWith(Instructions.Do, StringComparison.OrdinalIgnoreCase);

    public void Parse(ParserContext context, string current)
    {
      if (current.StartsWith(Instructions.Report, StringComparison.OrdinalIgnoreCase))
      {
        var parts = current.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length > 1)
          context.Report.ReportDefinition.Code = parts[1];

        if (parts.Length > 2)
          context.Report.ReportDefinition.Options.AddRange(parts.Skip(2).Select(x => new Option { Value = x }));
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

  internal class CodeBehindParser : IInstructionParser
  {
    public bool CanParse(string current, CodeContext context) => context == CodeContext.CodeBehind;

    public void Parse(ParserContext context, string current)
    {
      if (context.CurrentItem is Report report)
      {
        report.CodeBehind.Add(new Expression { Value = current });
      }
    }
  }
}
