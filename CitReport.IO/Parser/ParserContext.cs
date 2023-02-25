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

  public void MoveNext() => ++CurrentLine;
}
