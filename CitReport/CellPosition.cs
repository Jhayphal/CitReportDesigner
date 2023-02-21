using System.Diagnostics.CodeAnalysis;

namespace CitReport
{
  public readonly struct CellPosition
  {
    public CellPosition(int x, int y)
    {
      X = x;
      Y = y;
    }

    public int X { get; init; }

    public int Y { get; init; }

    public override int GetHashCode() => (Y << 16) | X;

    public override bool Equals([NotNullWhen(true)] object obj)
      => obj is CellPosition position && position.X == X && position.Y == Y;

    public override string ToString() => $"{X}:{Y}";

    public static bool operator ==(CellPosition left, CellPosition right) => left.Equals(right);

    public static bool operator !=(CellPosition left, CellPosition right) => !(left == right);

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
}