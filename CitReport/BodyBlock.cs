namespace CitReport
{
  public class BodyBlock : Block
  {
    public readonly List<Metadata> Metadata = new();

    public readonly Fonts Fonts = new();

    public float Height => (Options?.OfType<BlockHeightOption>().FirstOrDefault()?.Height).GetValueOrDefault();
  }
}