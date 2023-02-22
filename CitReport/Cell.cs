using System.Drawing;

namespace CitReport;

public sealed class Cell : IMultilanguageValueStorage, IEquatable<Cell>
{
  private static int lastId = 0;

  private readonly int id = ++lastId;

  private readonly List<Cell> children = new();

  public Cell(Table table)
  {
    Table = table;
  }

  public Table Table { get; }
  
  public Cell Parent { get; private set; }

  public IEnumerable<Cell> Children => children;

  public FontInfo Font { get; set; }

  public Color ForegroundColor { get; set; }

  public Color BackgroundColor { get; set; }

  public HorizontalAlignment HorizontalAlignment { get; set; }

  public VerticalAlignment VerticalAlignment { get; set; }

  public Dictionary<string, string> DisplayValue { get; } = new Dictionary<string, string>();

  public Dictionary<string, string> Value { get; } = new Dictionary<string, string>();

  public int Column => Table.GetCellColumnIndex(this);

  public int Row => Table.GetCellColumnIndex(this);

  public CellPosition Position => new(Column, Row);

  public float Width => Table.GetColumnWidth(Column);

  public float Height => Table.GetRowHeight(Row);

  public bool CanGrow { get; set; }

  public bool IsMerged => Parent != null || children.Count > 0;

  public void AddChild(Cell cell)
  {
    if (cell != null)
    {
      children.Add(cell);
      cell.Parent = this;
    }
  }

  public string GetValue(string language) => Value.TryGetValue(language, out var value)
    ? value
    : null;

  public void SetAsParent(Cell cell)
  {
    cell?.children.Add(this);
    Parent = cell;
  }

  private bool RemoveChild(Cell cell) => children.Remove(cell);

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

  public void SetValue(string language, string value) => Value[language] = value;

  public override int GetHashCode() => id;

  public override bool Equals(object obj) => Equals(obj as Cell);

  public bool Equals(Cell other)
    => other != null
      && ReferenceEquals(other.Table, Table)
      && other.Column == Column
      && other.Row == Row;

  public override string ToString() => $"{Column}:{Row}";

  public static bool operator ==(Cell left, Cell right) => left != null && left.Equals(right);

  public static bool operator !=(Cell left, Cell right) => !(left == right);
}