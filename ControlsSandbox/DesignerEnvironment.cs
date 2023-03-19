using System;

namespace ControlsSandbox;

public class DesignerEnvironment
{
  private string language = string.Empty;

  public static readonly DesignerEnvironment Current = new();

  public string Language
  {
    get => language;
    set
    {
      if (language != value)
      {
        language = value;
        LanguageChanged?.Invoke(this, language);
      }
    }
  }

  public event EventHandler<string> LanguageChanged;
}
