namespace CitReport;

public interface IMultilanguageValueStorage
{
  void SetValue(string language, string value);

  string GetValue(string language);
}