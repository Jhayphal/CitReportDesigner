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
      block = new BodyBlock(report, id: 1) { Code = "PH" };
      block.Options.Add(new BlkHOption { Height = 30 });
    }

    public BlockViewModel(BodyBlock block, int id)
    {
      this.block = block;
      Id = id;
    }

    public string Type => block.Code;

    public int Id
    {
      get => block.Id;
      set => block.Id = value;
    }

    public int ItemsCount => (block.Tables?.Count).GetValueOrDefault();

    public string Header => $"{Type} ({Id}) - {GetBlockTypeText()} [{GetItemsText()}]";

    public double HeaderFont => MeasurementConverter.MillimetersToPixels(3f);

    public double HeaderHeight => MeasurementConverter.MillimetersToPixels(5f);

    public double SummaryHeight => HeaderHeight + Height;

    public double Width { get; set; } = MeasurementConverter.MillimetersToPixels(190f);

    public double Height => MeasurementConverter.MillimetersToPixels(block.Height);

    private string GetItemsText() => ItemsCount switch
    {
      1 => $"{ItemsCount} элемент",
      2 or 3 or 4 => $"{ItemsCount} элемента",
      _ => $"{ItemsCount} элементов",
    };

    private string GetBlockTypeText()
      => descriptions.TryGetValue(Type.ToUpper(), out string description)
        ? description
        : "Неизвестный тип";
  }
}
