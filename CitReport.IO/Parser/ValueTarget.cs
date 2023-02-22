namespace CitReport.IO;

internal readonly struct ValueTarget
{
  public ValueTarget(IMultilanguageValueStorage target, string language)
  {
    Target = target;
    Language = language;
  }

  public readonly IMultilanguageValueStorage Target;

  public readonly string Language;
}
