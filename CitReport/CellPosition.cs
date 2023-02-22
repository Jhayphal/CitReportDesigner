using System.Diagnostics.CodeAnalysis;

namespace CitReport;

public readonly struct CellPosition : IEquatable<CellPosition>, IComparable<CellPosition>
{
  public CellPosition(int x, int y)
  {
    if (x < 0 || y < 0)
    {
      throw new ArgumentOutOfRangeException(x < 0 ? nameof(x) : nameof(y));
    }

    X = x;
    Y = y;
  }

  public readonly int X;

  public readonly int Y;

  public override int GetHashCode() => (Y << 16) | X;

  public override bool Equals([NotNullWhen(true)] object obj) => obj is CellPosition other && Equals(other);

  public bool Equals(CellPosition other) => other.X == X && other.Y == Y;

  public int CompareTo(CellPosition other)
  {
    var result = X.CompareTo(other.X);
    if (result != 0)
    {
      return result;
    }

    return Y.CompareTo(other.Y);
  }

  public override string ToString() => $"{X}:{Y}";

  public static bool operator ==(CellPosition left, CellPosition right) => left.Equals(right);

  public static bool operator !=(CellPosition left, CellPosition right) => !(left == right);

  public static bool operator <(CellPosition left, CellPosition right) => left.CompareTo(right) < 0;

  public static bool operator <=(CellPosition left, CellPosition right) => left.CompareTo(right) <= 0;

  public static bool operator >(CellPosition left, CellPosition right) => left.CompareTo(right) > 0;

  public static bool operator >=(CellPosition left, CellPosition right) => left.CompareTo(right) >= 0;

  public static CellPosition FromString(string position)
  {
    if (string.IsNullOrEmpty(position))
    {
      return new CellPosition(0, 0);
    }

    var index = 0;
    while (index < position.Length && char.IsWhiteSpace(position[index]))
    {
      ++index;
    }

    if (!char.IsLetter(position[index]))
    {
      return new CellPosition(0, 0);
    }

    var x = ColumnToIndex(position[index++]);

    if (index < position.Length)
    {
      return new CellPosition(x, 0);
    }

    _ = int.TryParse(position[index..], out var y);
    
    return new CellPosition(x, y);
  }

  public static string ToString(CellPosition position) => $"{IndexToColumn(position.X)}{position.Y + 1}";

  public static int ColumnToIndex(char column) => column - (char.IsUpper(column) ? 'A' : 'a');

  public static char IndexToColumn(int column) => (char)('A' + column);
}