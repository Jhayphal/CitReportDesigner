namespace CitReport;

public interface IMultilanguageValueStorage
{
  void AddValue(string language, string value);

  void SetValue(string language, int index, string value);

  string GetValue(string language, int index);
}