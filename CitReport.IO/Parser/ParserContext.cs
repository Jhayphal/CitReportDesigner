namespace CitReport.IO;

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

  public readonly Queue<ValueTarget> Fields = new();

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
