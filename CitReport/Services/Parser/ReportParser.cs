using System.Drawing;

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

  internal sealed class ParserContext
  {
    public ParserContext(IErrorProvider errorProvider)
    {
      ErrorProvider = errorProvider;
      Report = new Report();
      Context = CodeContext.CodeBehind;
    }

    public Report Report { get; }
    
    public CodeContext Context { get; private set; }

    public BodyBlock CurrentBlock { get; private set; }

    public IErrorProvider ErrorProvider { get; }

    public readonly Queue<IMultilanguageValueStorage> Fields = new();

    public void SetContext(CodeContext context)
    {
      Context = context;
    }

    public void SetBlockAsCurrent(BodyBlock current)
    {
      if (Fields.Count > 0)
      {
        ErrorProvider.AddError($"{Fields.Count} fields has not values.");
        Fields.Clear();
      }

      CurrentBlock = current;
    }
  }

  internal class MetadataParser : IInstructionParser
  {
    public bool CanParse(string current, CodeContext context) => current.StartsWith("{/*");

    public void Parse(ParserContext context, string current)
    {
      var metadata = context.CurrentBlock == null
        ? context.Report.Metadata
        : context.CurrentBlock.Metadata;

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
    public const string Fl = "/FL";
    public const string Tc = "/TC";
    public const string Tr = "/TR";
    public const string Ts = "/TS";
    public const string Tsg = "/TSG";
    public const string Ct = "/CT";
  }

  internal class TableParser : BlockInstructionParser
  {
    private float[] columns;
    private float[] rows;
    private Table table;
    private Color foregroundColor = Color.Black;
    private Color backgroundColor = Color.White;

    private readonly string[] supportedInstructions = new string[] 
    { 
      Instructions.Tc, 
      Instructions.Tr, 
      Instructions.Ts 
    };

    private readonly string[] fieldInstructions = new string[]
    {
      Instructions.Ts,
      Instructions.Tsg
    };

    protected override CodeContext ActualContext => CodeContext.Block;

    protected override IEnumerable<string> SupportedInstructions => throw new NotImplementedException();

    public override void Parse(ParserContext context, string current)
    {
      if (IsInstructionSupported(current, Instructions.Tc))
      {
        columns = ParseArray(Instructions.Tc, current);
        CreateTableIfRequired(context);
      }
      else if (IsInstructionSupported(current, Instructions.Tr))
      {
        rows = ParseArray(Instructions.Tr, current);
        CreateTableIfRequired(context);
      }
      else if (table != null)
      {
        if (IsInstructionSupported(current, fieldInstructions))
        {
          ParseCell(context, current);
        }
        else if (IsInstructionSupported(current, Instructions.Ct))
        {
          ParseForegroundColor(context, current);
        }
      }
      else
      {
        context.ErrorProvider.AddError($"Unexpected table instruction outside table definition.");
      }
    }

    /// <summary>
    /// Parse text color.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="current"></param>
    /// <remarks>{/CT, 0, 128, 128}</remarks>
    private void ParseForegroundColor(ParserContext context, string current)
    {
      var tokens = Tokenizer.GetTokens(current)
        .Select(x => x.Trim())
        .Where(x => byte.TryParse(x, out _))
        .Select(x => byte.Parse(x))
        .ToArray();

      foregroundColor = Color.FromArgb(tokens[0], tokens[1], tokens[2]);
    }

    /// <summary>
    /// Parse table cell.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="current"></param>
    /// <remarks>For example, {/TS, A2:A2, , FONT1}Field ___</remarks>
    private void ParseCell(ParserContext context, string current)
    {
      var tokens = Tokenizer.GetTokens(current)
        .Select(x => x.Trim())
        .ToList();
      var endDefinitionIndex = tokens.IndexOf("}");
      if (endDefinitionIndex < 0 || tokens.Count < 11)
      {
        context.ErrorProvider.AddError($"Unfinished instruction: '{current}'.");
        return;
      }

      var canGrow = string.Equals(tokens[1], "/TSG", StringComparison.OrdinalIgnoreCase);
      var languageSpecifed = tokens[2] == ":";
      var language = languageSpecifed ? tokens[3] : string.Empty;
      var offset = languageSpecifed ? 2 : 0;
      var cellPositionLeftUpper = CellPosition.FromString(tokens[3 + offset].Trim());
      var cellPositionRightBottom = CellPosition.FromString(tokens[5 + offset].Trim());
      Cell cell = table.Merge(cellPositionLeftUpper, cellPositionRightBottom);
      cell.CanGrow = canGrow;
      var displayValueIndex = endDefinitionIndex + 1;
      var displayValue = displayValueIndex >= tokens.Count
        ? string.Empty
        : tokens[displayValueIndex];
      cell.DisplayValue[language] = displayValue;

      if (displayValue.Contains('_'))
      {
        context.Fields.Enqueue(cell);
      }

      var fontAlias = tokens[9 + offset].Trim();
      if (context.CurrentBlock.Fonts.TryGetValue(fontAlias, out var fontInfo))
      {
        cell.Font = fontInfo;
      }
      else
      {
        context.ErrorProvider.AddError($"Undefined font '{fontAlias}'.");
      }

      cell.ForegroundColor = foregroundColor;
      cell.BackgroundColor = backgroundColor;
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

  internal sealed class FontParser : BlockInstructionParser
  {
    private readonly string[] supportedInstructions = new string[] { Instructions.Fl };

    protected override CodeContext ActualContext => CodeContext.Block;

    protected override IEnumerable<string> SupportedInstructions => supportedInstructions;

    /// <summary>
    /// Parse font.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="current"></param>
    /// <remarks>{/FL, FONT1, Times New Roman, 10, B}</remarks>
    public override void Parse(ParserContext context, string current)
    {
      var parts = Tokenizer.GetTokens(current)
        .Select(x => x.Trim())
        .ToList();

      if (parts.Count < 8)
      {
        context.ErrorProvider.AddError($"Unfinished instruction: '{current}'.");
        return;
      }

      var alias = parts[3];

      var font = new FontInfo
      {
        Family = parts[5],
        Size = float.TryParse(parts[7], out var size) ? size : 0
      };

      if (parts.Count > 9)
      {
        font.Style = string.Equals(parts[9], "B", StringComparison.OrdinalIgnoreCase)
          ? FontStyle.Bold
          : FontStyle.Regular;
      }

      context.CurrentBlock.Fonts.Add(alias, font);
    }
  }
}
