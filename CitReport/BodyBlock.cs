using CitReport.Extensions;

namespace CitReport;

public class BodyBlock : Block
{
  public readonly List<Metadata> Metadata = new();

  public readonly Dictionary<string, FontInfo> Fonts = new();

  public float Height => (Options?.OfType<BlkHOption>().FirstOrDefault()?.Height).GetValueOrDefault();

  public override bool Equals(Block other)
    => base.Equals(other)
      && other is BodyBlock block
      && Height == block.Height
      && Metadata.AreEquals(block.Metadata)
      && Fonts.AreEquals(block.Fonts);
}