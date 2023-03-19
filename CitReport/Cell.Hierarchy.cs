namespace CitReport;

public sealed partial class Cell
{
  private readonly List<Cell> children = new();

  public Cell Parent { get; private set; }

  public IEnumerable<Cell> Children => children;

  public void AddChild(Cell cell)
  {
    if (cell != null)
    {
      children.Add(cell);
      cell.Parent = this;
    }
  }

  public void SetAsParent(Cell cell)
  {
    cell?.children.Add(this);
    Parent = cell;
  }

  private bool RemoveChild(Cell cell) => children.Remove(cell);
}
