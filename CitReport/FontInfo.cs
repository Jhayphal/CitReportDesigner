namespace CitReport;

public class FontInfo : IEquatable<FontInfo>
{
  public string Family { get; set; }

  public float Size { get; set; }

  public FontStyle Style { get; set; }

  public bool Equals(FontInfo other)
    => other is not null
      && other.Family == Family
      && other.Size == Size
      && other.Style == Style;

  public override bool Equals(object obj) => Equals(obj as FontInfo);

  public override int GetHashCode() => Family.GetHashCode() ^ Size.GetHashCode() ^ Style.GetHashCode();

  public override string ToString() => $"{Family} {Size} {Style}";

  public static bool operator ==(FontInfo left, FontInfo right) => left is null
    ? right is null
    : left.Equals(right);

  public static bool operator !=(FontInfo left, FontInfo right) => !(left == right);
}