using System.Drawing;
using System.Text;

namespace CitReport
{
  public class Cell : IMultilanguageValueStorage
  {
    protected readonly List<Cell> children = new();

    public Cell(Table table)
    {
      Table = table;
    }

    public Table Table { get; }
    
    public Cell Parent { get; protected set; }

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

    public void AddChild(Cell cell)
    {
      children.Add(cell);
      cell.Parent = this;
    }

    public string GetValue(string language) => Value.TryGetValue(language, out var value)
      ? value
      : null;

    public void SetAsParent(Cell cell)
    {
      cell.children.Add(this);
      Parent = cell;
    }

    public void SetValue(string language, string value) => Value[language] = value;
  }
}