using CitReport;

namespace ControlsSandbox.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
  public MainWindowViewModel()
  {
    var report = new Report();

    report.Metadata.Add(new Metadata { Value = "PAGESIZE 297x420" });
    report.Metadata.Add(new Metadata { Value = "MARGL 10" });
    report.Metadata.Add(new Metadata { Value = "MARGT 10" });
    report.Metadata.Add(new Metadata { Value = "MARGR 10" });
    report.Metadata.Add(new Metadata { Value = "MARGB 10" });

    report.Definition.Code = "ZP1PN1";
    report.Definition.Options.Add(new BreakOption { Name = "BREAK", Value = "STR(KODR,2)" });

    var block = new BodyBlock(report, id: 1) { Code = "PH" };
    report.Blocks.Add(block);

    block.Options.Add(new BlkHOption { Value = "0" });

    block = new BodyBlock(report, id: 2) { Code = "RH" };
    report.Blocks.Add(block);

    block.Options.Add(new BlkHOption { Value = "30" });
    block.Options.Add(new New_LinebreakOption());

    var fontAlias = "FONT1";
    block.Fonts.Add(fontAlias, new FontInfo { Family = "Times New Roman", Size = 10f });

    block.Metadata.Add(new Metadata { Value = "BlockHeaderColor:=227,197,186" });

    block.TextBlocks.Add(new TextBlock
    {
      Text = "Текст для отчета",
      X = 10,
      Y = 10,
      Width = 50,
      Height = 5,
      HorizontalAlignment = HorizontalAlignment.Center,
      VerticalAlignment = VerticalAlignment.Center
    });

    block = new BodyBlock(report, id: 3) { Code = "RH" };
    report.Blocks.Add(block);

    block.Options.Add(new BlkHOption { Value = "50" });
    block.Options.Add(new New_LinebreakOption());

    fontAlias = "FONT1";
    block.Fonts.Add(fontAlias, new FontInfo { Family = "Times New Roman", Size = 10f });

    block.Metadata.Add(new Metadata { Value = "BlockHeaderColor:=227,197,186" });

    var table = new Table(new double[] { 0, 50, 100, 150 }, new double[] { 0, 5, 10 });
    block.Tables.Add(table);

    var cell = table.GetCell(0, 0);
    cell.Properties.DisplayValue.Add(string.Empty, "Имя");
    cell.Style.VerticalAlignment = VerticalAlignment.Center;
    cell.Style.HorizontalAlignment = HorizontalAlignment.Center;
    cell.Style.Font = block.Fonts[fontAlias];

    cell = table.GetCell(1, 0);
    cell.Properties.DisplayValue.Add(string.Empty, "Фамилия");
    cell.Style.VerticalAlignment = VerticalAlignment.Center;
    cell.Style.HorizontalAlignment = HorizontalAlignment.Center;
    cell.Style.Font = block.Fonts[fontAlias];

    cell = table.GetCell(2, 0);
    cell.Properties.DisplayValue.Add(string.Empty, "Отчество");
    cell.Style.VerticalAlignment = VerticalAlignment.Center;
    cell.Style.HorizontalAlignment = HorizontalAlignment.Center;
    cell.Style.Font = block.Fonts[fontAlias];

    cell = table.GetCell(0, 1);
    cell.Properties.DisplayValue.Add(string.Empty, "Олег");
    cell.Style.VerticalAlignment = VerticalAlignment.Center;
    cell.Style.HorizontalAlignment = HorizontalAlignment.Center;
    cell.Style.Font = block.Fonts[fontAlias];

    cell = table.GetCell(1, 1);
    cell.Properties.DisplayValue.Add(string.Empty, "Назаренко");
    cell.Style.VerticalAlignment = VerticalAlignment.Center;
    cell.Style.HorizontalAlignment = HorizontalAlignment.Center;
    cell.Style.Font = block.Fonts[fontAlias];

    cell = table.GetCell(2, 1);
    cell.Properties.DisplayValue.Add(string.Empty, "Сергеевич");
    cell.Style.VerticalAlignment = VerticalAlignment.Center;
    cell.Style.HorizontalAlignment = HorizontalAlignment.Center;
    cell.Style.Font = block.Fonts[fontAlias];
    
    Content = new ReportViewModel(report);
  }

  public ReportViewModel Content { get; set; }
}
