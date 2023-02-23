namespace CitReport;

public class Report
{
  public readonly List<Metadata> Metadata = new();

  public readonly List<Expression> CodeBehind = new();

  public readonly ReportBlock Definition = new();

  public readonly List<BodyBlock> Blocks = new();
}