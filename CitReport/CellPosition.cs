using System.Diagnostics.CodeAnalysis;

namespace CitReport;

public readonly struct CellPosition : IEquatable<CellPosition>, IComparable<CellPosition>
{
  public CellPosition(int column, int row)
  {
    if (column < 0 || row < 0)
    {
      throw new ArgumentOutOfRangeException(column < 0 ? nameof(column) : nameof(row));
    }

    Column = column;
    Row = row;
  }

  public readonly int Column;

  public readonly int Row;

  public override int GetHashCode() => (Row << 16) | Column;

  public override bool Equals([NotNullWhen(true)] object obj) => obj is CellPosition other && Equals(other);

  public bool Equals(CellPosition other) => other.Column == Column && other.Row == Row;

  public int CompareTo(CellPosition other)
  {
    var result = Column.CompareTo(other.Column);
    if (result != 0)
    {
      return result;
    }

    return Row.CompareTo(other.Row);
  }

  public override string ToString() => $"{Column}:{Row}";

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

  public static string ToString(CellPosition position) => $"{IndexToColumn(position.Column)}{position.Row + 1}";

  public static int ColumnToIndex(char column) => column - (char.IsUpper(column) ? 'A' : 'a');

  public static char IndexToColumn(int column) => (char)('A' + column);
}