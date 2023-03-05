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

    public string Type { get; set; } = "PH";

    public int Id { get; set; } = 1;

    public int ItemsCount { get; set; } = 2;

    public string Header => $"{Type} ({Id}) - {(descriptions.TryGetValue(Type, out var description) ? description : "Неизвестный тип")} [{ItemsCount} элемента]";

    public float Width { get; set; } = 300;

    public float Height { get; set; } = 100f;
  }
}
