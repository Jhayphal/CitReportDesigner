namespace CitReport;

public sealed partial class Cell : IMultilanguageValueStorage, IEquatable<Cell>
{
  private static int lastId = 0;

  private readonly int id = ++lastId;

  public Cell(Table table)
  {
    Table = table;
  }

  public readonly Table Table;

  public readonly CellProperties Properties = new();

  public readonly CellStyle Style = new();

  public int Column => Table.GetColumnIndex(this);

  public int Row => Table.GetRowIndex(this);

  public CellPosition Position => new(Column, Row);

  public double X
  {
    get => Table.GetCellX(this);
    set => Table.SetCellX(this, value);
  }

  public double Y
  {
    get => Table.GetCellY(this);
    set => Table.SetCellY(this, value);
  }

  public double Width
  {
    get => Table.GetColumnWidth(Column);
    set => Table.SetColumnWidth(Column, value);
  }

  public double Height
  {
    get => Table.GetRowHeight(Row);
    set => Table.SetRowHeight(Row, value);
  }

  public bool IsMerged => Parent != null || children.Count > 0;

  public void Break()
  {
    if (Parent != null)
    {
      Parent.RemoveChild(this);
      Parent = null;
    }

    foreach (Cell cell in children)
    {
      cell.Break();
    }
  }

  public void AddValue(string language, string value) => Properties.AddValue(language, value);

  public string GetValue(string language, int index) => Properties.GetValue(language, index);

  public void SetValue(string language, int index, string value) => Properties.SetValue(language, index, value);

  public override int GetHashCode() => id;

  public override bool Equals(object obj) => Equals(obj as Cell);

  public bool Equals(Cell other) => other.GetHashCode() == GetHashCode();

  public override string ToString() => $"{Column}:{Row}";

  public static bool operator ==(Cell left, Cell right) => left is null
    ? right is null
    : left.Equals(right);

  public static bool operator !=(Cell left, Cell right) => !(left == right);
}