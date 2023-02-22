namespace CitReport.IO.Parser;

public readonly struct ValueTarget
{
  public ValueTarget(IMultilanguageValueStorage target, string language)
  {
    Target = target;
    Language = language;
  }

  public readonly IMultilanguageValueStorage Target;

  public readonly string Language;
}
