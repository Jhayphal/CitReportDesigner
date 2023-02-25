using System.Drawing;

namespace CitReport.IO.Parser;

public sealed class ParserContext
{
  public ParserContext(IErrorProvider errorProvider)
  {
    ErrorProvider = errorProvider;
    Report = new Report();
    CodeContext = CodeContext.CodeBehind;
  }

  public Report Report { get; }
  
  public CodeContext CodeContext { get; private set; }

  public BodyBlock CurrentBlock { get; private set; }

  #region Table Context
  public Table CurrentTable { get; private set; }

  public float[] Columns { get; set; }
  
  public float[] Rows { get; set; }
  
  public Color CellForegroundColor { get; set; } = Color.Black;
  
  public Color CellBackgroundColor { get; set; } = Color.White;
  #endregion

  public int CurrentLine { get; private set; }

  public IErrorProvider ErrorProvider { get; }

  public readonly Queue<ValueTarget> Fields = new();

  public void SetContext(CodeContext context) => CodeContext = context;

  public void SetBlockAsCurrent(BodyBlock current)
  {
    if (Fields.Count > 0)
    {
      ErrorProvider.SomeFieldsHasNotValues(Fields.Count, Report.Blocks.Count);
      Fields.Clear();
    }

    CurrentBlock = current;
  }

  public void SetTableAsCurrent(Table current) => CurrentTable = current;

  public void MoveNext() => ++CurrentLine;
}
