using CitReport;
using System.Collections.Generic;

namespace ControlsSandbox.ViewModels
{
  public class BlockViewModel : ViewModelBase
  {
    private static Dictionary<string, string> descriptions = new()
    {
      { "RH", "Заголовок отчета" },
      { "PH", "Заголовок страницы" },
      { "IH", "Заголовок таблицы" },
      { "CH", "Начало прерывания" },
      { "DT", "Детальный блок" },
      { "CF", "Окончание прерывания" },
      { "PF", "Окончание страницы" },
      { "RF", "Окончание отчета" }
    };

    private readonly BodyBlock block;

    public BlockViewModel()
    {
      var report = new Report();
      this.block = new BodyBlock(report) { Code = "PH" };
      block.Options.Add(new BlkHOption { Height = 30 });
    }

    public BlockViewModel(BodyBlock block, int id)
    {
      this.block = block;
      Id = id;
    }

    public string Type => this.block.Code;

    public int Id { get; set; } = 1;

    public int ItemsCount => (block.Tables?.Count).GetValueOrDefault();

    public string Header => $"{Type} ({Id}) - {(descriptions.TryGetValue(Type, out var description) ? description : "Неизвестный тип")} [{ItemsCount} элемента]";

    public float HeaderHeight => MillimetersToPixels(5f);

    public float SummaryHeight => HeaderHeight + Height;

    public float Width { get; set; } = MillimetersToPixels(190f);

    public float Height => MillimetersToPixels(block.Height);
  }
}
