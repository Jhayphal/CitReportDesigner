namespace CitReport;

public class CellProperties : IMultilanguageValueStorage
{
  public bool CanGrow { get; set; }

  public Dictionary<string, string> DisplayValue { get; } = new();

  public Dictionary<string, List<string>> Value { get; } = new();

  public void AddValue(string language, string value)
  {
    if (!Value.TryGetValue(language, out var values))
    {
      Value.Add(language, values = new());
    }

    values.Add(value);
  }

  public string GetValue(string language, int index)
    => Value.TryGetValue(language, out var value)
      ? value[index]
      : null;

  public void SetValue(string language, int index, string value)
    => Value[language][index] = value;
}