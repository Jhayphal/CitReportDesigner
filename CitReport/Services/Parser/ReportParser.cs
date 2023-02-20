using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

  internal class OptionsParser
  {
    public IEnumerable<Option> Parse(string current, IErrorProvider errorProvider)
    {
      var result = new List<Option>();
      var tokenizer = new Tokenizer(current);

      foreach (var option in tokenizer.GetTokens().Chunk(2))
      {
        var optionName = option[0];

        if (option.Length < 2)
        {
          errorProvider.AddError($"Option '{optionName}' has not value.");
          break;
        }

        if (!Options.Map.TryGetValue(optionName.ToLower(), out var optionType))
        {
          errorProvider.AddError($"Unsupported option '{optionName}'.");
          optionType = typeof(Option);
        }

        var optionValue = Activator.CreateInstance(optionType) as Option;
        if (optionValue == null)
        {
          errorProvider.AddError($"Type '{optionType.FullName}' does not inherit Option type.");
        }
        else
        {
          optionValue.Name = optionName;
          optionValue.Value = option[2];
          result.Add(optionValue);
        }
      }

      return result;
    }
  }

  internal class Tokenizer
  {
    private readonly string text;
    private readonly StringBuilder builder = new(); 
    private int position;

    public Tokenizer(string text)
    {
      this.text = text;
    }

    public IEnumerable<string> GetTokens()
    {
      while (position < text.Length)
      {
        while (position < text.Length && char.IsWhiteSpace(text[position]))
        {
          ++position;
        }

        if (position >= text.Length)
        {
          yield break;
        }

        while (position < text.Length && text[position] != '(')
        {
          builder.Append(text[position]);
        }

        var token = builder.ToString();
        builder.Clear();

        yield return token;

        int brackects = 1;

        while (++position < text.Length && brackects > 0)
        {
          if (text[position] == '(')
          {
            ++brackects;
          }
          else if (text[position] == ')')
          {
            --brackects;
          }

          if (brackects > 0)
          {
            builder.Append(text[position]);
          }
        }

        token = builder.ToString();
        builder.Clear();

        yield return token;
      }
    }
  }
}
