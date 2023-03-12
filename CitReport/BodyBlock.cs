using CitReport.Extensions;

namespace CitReport;

public class BodyBlock : Block
{
  public BodyBlock(Report report, int id)
  {
    Report = report;
    Id = id;
  }

  public readonly Report Report;

  public int Id { get; set; }

  public readonly List<Metadata> Metadata = new();

  public readonly Dictionary<string, FontInfo> Fonts = new();

  public readonly List<Table> Tables = new();

  public readonly List<TextBlock> TextBlocks = new();

  public float Height => (Options?.OfType<BlkHOption>().FirstOrDefault()?.Height).GetValueOrDefault();

  public override bool Equals(Block other)
    => base.Equals(other)
      && other is BodyBlock block
      && Id == block.Id
      && Height == block.Height
      && Metadata.AreEquals(block.Metadata)
      && Fonts.AreEquals(block.Fonts);
}