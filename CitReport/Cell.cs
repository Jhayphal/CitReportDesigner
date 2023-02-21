namespace CitReport
{
  public class Cell
  {
    protected readonly List<Cell> children = new();

    public Cell(Table table)
    {
      Table = table;
    }

    public Table Table { get; }
    
    public Cell Parent { get; protected set; }

    public IEnumerable<Cell> Children => children;

    public Dictionary<string, string> DisplayValue { get; } = new Dictionary<string, string>();

    public Dictionary<string, string> Value { get; } = new Dictionary<string, string>();

    public int Column => Table.GetCellColumnIndex(this);

    public int Row => Table.GetCellColumnIndex(this);

    public float Width => Table.GetColumnWidth(Column);

    public float Height => Table.GetRowHeight(Row);

    public bool MayGrow { get; set; }

    public void AddChild(Cell cell)
    {
      children.Add(cell);
      cell.Parent = this;
    }

    public void SetAsParent(Cell cell)
    {
      cell.children.Add(this);
      Parent = cell;
    }
  }
}