using System.Text;

namespace CitReport.Services.Parser
{
  internal sealed class ReportParser
  {
    private readonly IEnumerable<IInstructionParser> parsers = new List<IInstructionParser>
    {
      new MetadataParser(),
      new ReportDefinitionParser(),
      new BodyBlockParser(),
      new FontParser(),
      
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
          break;
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
    public const string Blk = "/BLK ";
    public const string Fl = "{/FL";
    public const string Tc = "{/TC";
    public const string Tr = "{/TR";
    public const string Ts = "{/TS";
    public const string Tsg = "{/TSG";
  }

  internal class TableParser : IInstructionParser
  {
    private float[] columns;
    private float[] rows;
    private Table table;

    private readonly BlockInstructionTokenizer tokenizer = new();

    public bool CanParse(string current, CodeContext context)
      => context == CodeContext.Block
        && IsInstructionSupported(Instructions.Tc, Instructions.Tr, Instructions.Ts);

    private bool IsInstructionSupported(string current, params string[] instructions)
      => instructions.Any(x => current.StartsWith(current, StringComparison.OrdinalIgnoreCase));

    public void Parse(ParserContext context, string current)
    {
      if (current.StartsWith(Instructions.Tc, StringComparison.OrdinalIgnoreCase))
      {
        columns = ParseArray(Instructions.Tc, current);
        CreateTableIfRequired(context);
      }
      else if (current.StartsWith(Instructions.Tr, StringComparison.OrdinalIgnoreCase))
      {
        rows = ParseArray(Instructions.Tr, current);
        CreateTableIfRequired(context);
      }
      else if (table != null)
      {
        if (current.StartsWith(Instructions.Ts, StringComparison.OrdinalIgnoreCase))
        {
          ParseCell(context, current);
        }
      }
    }

    private void ParseCell(ParserContext context, string current)
    {
      var cell = new Cell(table);

      var enumerator = tokenizer.GetTokens(current).GetEnumerator();
      var token = string.Empty;
      while (TryMoveEnumerator(context, current, enumerator))
      {
        token = enumerator.Current;

        if (!string.IsNullOrWhiteSpace(token) && token != "{")
        {
          break;
        }
      }
      
      if (enumerator.Current == null)
      {
        return;
      }

      if (string.Equals(token, Instructions.Ts, StringComparison.OrdinalIgnoreCase))
      {
        cell.MayGrow = false;
      }
      else if (string.Equals(token, Instructions.Tsg, StringComparison.OrdinalIgnoreCase))
      {
        cell.MayGrow = true;
      }
      else
      {
        context.ErrorProvider.AddError($"Unexpected instruction: '{token}'.");
        return;
      }

      if (!TryMoveEnumerator(context, current, enumerator))
      {
        return;
      }

      var language = string.Empty;
      token = enumerator.Current;
      if (token == ":")
      {
        if (!TryMoveEnumerator(context, current, enumerator))
        {
          return;
        }

        token = enumerator.Current;
        language = token;

        if (!TryMoveEnumerator(context, current, enumerator))
        {
          return;
        }
      }

      if (token != ",")
      {
        context.ErrorProvider.AddError($"Unexpected instruction: '{token}'.");
        return;
      }

      if (!TryMoveEnumerator(context, current, enumerator))
      {
        return;
      }
    }

    private bool TryMoveEnumerator(ParserContext context, string current, IEnumerator<string> enumerator)
    {
      if (!enumerator.MoveNext())
      {
        context.ErrorProvider.AddError($"Unfinished instruction: '{current}'.");
        return false;
      }

      return true;
    }

    private void CreateTableIfRequired(ParserContext context)
    {
      table = null;

      if (columns != null && rows != null)
      {
        if (columns.Length == 0)
        {
          context.ErrorProvider.AddError("Table has not columns.");
        }

        if (rows.Length == 0)
        {
          context.ErrorProvider.AddError("Table has not rows.");
        }

        table = new Table(columns, rows);
        columns = null;
        rows = null;
      }
    }

    private float[] ParseArray(string instruction, string current)
      => current[instruction.Length..].TrimEnd('}')
        .Split(',', StringSplitOptions.RemoveEmptyEntries)
        .Where(x => float.TryParse(x, out var _))
        .Select(float.Parse)
        .ToArray();
  }

  internal class BlockInstructionTokenizer
  {
    private static readonly HashSet<char> breakers = new() { '{', ',', ':', '}' };

    public IEnumerable<string> GetTokens(string current)
    {
      if (current == null)
      {
        yield break;
      }

      var builder = new StringBuilder();
      var position = 0;

      do
      {
        while (position < current.Length && !breakers.Contains(current[position]))
        {
          builder.Append(current[position++]);
        }

        if (builder.Length > 0)
        {
          var result = builder.ToString();
          builder.Clear();
          yield return result;
        }

        if (position < current.Length)
        {
          yield return current[position++].ToString();
        }
      }
      while (position < current.Length);
    }
  }

  internal class FontParser : IInstructionParser
  {
    public bool CanParse(string current, CodeContext context)
      => context == CodeContext.Block && current.StartsWith(Instructions.Fl, StringComparison.OrdinalIgnoreCase);

    public void Parse(ParserContext context, string current)
    {
      if (context.CurrentItem is not BodyBlock block)
      {
        context.ErrorProvider.AddError($"Font cannot be implemented in context '{context.Context}'.");
        return;
      }

      var parts = current.Split(' ', StringSplitOptions.RemoveEmptyEntries);
      var font = new FontInfo();

      if (parts.Length > 1)
      {
        font.Alias = parts[1];
      }

      if (parts.Length > 2)
      {
        font.Family = parts[2];
      }

      if (parts.Length > 3)
      {
        font.Size = float.TryParse(parts[3], out var size) ? size : 0;
      }

      if (parts.Length > 4)
      {
        font.Style = string.Equals(parts[4], "B", StringComparison.OrdinalIgnoreCase)
          ? FontStyle.Bold
          : FontStyle.Regular;
      }

      block.Fonts.Add(font);
    }
  }

  internal class BodyBlockParser : IInstructionParser
  {
    private readonly OptionsParser optionsParser = new();

    public bool CanParse(string current, CodeContext context) => current.StartsWith(Instructions.Blk, StringComparison.OrdinalIgnoreCase);

    public void Parse(ParserContext context, string current)
    {
      var block = new BodyBlock();

      var parts = current.Split(' ', StringSplitOptions.RemoveEmptyEntries);

      if (parts.Length > 1)
        block.Code = parts[1];

      if (parts.Length > 2)
      {
        var options = string.Join(" ", parts.Skip(2));
        block.Options.AddRange(optionsParser.Parse(options, context.ErrorProvider));
      }

      context.Report.BodyBlocks.Add(block);
      context.CurrentItem = block;
    }
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
        {
          var options = string.Join(" ", parts.Skip(2));
          var optionsParser = new OptionsParser();
          context.Report.ReportDefinition.Options.AddRange(optionsParser.Parse(options, context.ErrorProvider));
        }

        context.CurrentItem = context.Report.ReportDefinition;
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
