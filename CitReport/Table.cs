namespace CitReport
{
  public class Table
  {
    private readonly List<Cell> cells;
    private readonly List<float> columns;
    private readonly List<float> rows;

    public Table(IEnumerable<float> columns, IEnumerable<float> rows)
    {
      this.columns = new List<float>(columns);
      this.rows = new List<float>(rows);

      cells = new List<Cell>();

      var count = this.columns.Count * this.rows.Count;
      while (count-- > 0)
      {
        cells.Add(new Cell(this));
      }
    }

    public IEnumerable<float> Columns { get; }
    
    public IEnumerable<float> Rows { get; }

    public int GetCellRowIndex(Cell cell)
    {
      var index = cells.IndexOf(cell);
      if (index == -1)
      {
        return index;
      }
      
      return index / this.columns.Count;
    }

    public int GetCellColumnIndex(Cell cell)
    {
      var index = cells.IndexOf(cell);
      if (index == -1)
      {
        return index;
      }

      return index % this.columns.Count;
    }

    public float GetColumnWidth(int index) => this.columns[index];

    public float GetRowHeight(int index) => this.rows[index];

    public Cell GetCell(int column, int row) => cells[row * columns.Count + column];

    /// <summary>
    /// Merge cells by text range, if required.
    /// </summary>
    /// <param name="leftUpper">Like A3.</param>
    /// <param name="rightBottom">Like B8.</param>
    /// <returns>Merged cell.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public Cell Merge(CellPosition leftUpper, CellPosition rightBottom)
    {
      throw new NotImplementedException();
    }
  }
}