using CitReport.Extensions;

namespace CitReport;

public abstract class Block : IEquatable<Block>
{
  private static int lastId = 0;

  private static readonly int id = ++lastId;

  public string Code { get; set; }

  public readonly List<Option> Options = new();

  public virtual bool Equals(Block other)
  {
    if (other == null)
    {
      return false;
    }

    return other.Code == Code && other.Options.AreEquals(Options);
  }

  public override bool Equals(object obj) => Equals(obj as Block);

  public override int GetHashCode() => id;

  public override string ToString() => $"{Code} {string.Join(" ", Options)}";

  public static bool operator ==(Block left, Block right) => left is null
    ? right is null
    : left.Equals(right);

  public static bool operator !=(Block left, Block right) => !(left == right);
}