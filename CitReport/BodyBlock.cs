namespace CitReport;

public class BodyBlock : Block
{
  public readonly List<Metadata> Metadata = new();

  public readonly Dictionary<string, FontInfo> Fonts = new();

  public float Height => (Options?.OfType<BlkHOption>().FirstOrDefault()?.Height).GetValueOrDefault();
}