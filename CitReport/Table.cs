using System.Collections;

namespace CitReport;

public sealed class Table : IEnumerable<Cell>
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

  public IEnumerable<float> Columns => columns;
  
  public int ColumnsCount => columns.Count;

  public IEnumerable<float> Rows => rows;

  public int RowsCount => rows.Count;

  public IEnumerator<Cell> GetEnumerator()
    => GetRange(new CellPosition(0, 0), new CellPosition(ColumnsCount, RowsCount)).GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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

  public Cell GetCell(int column, int row)
  {
    var cell = GetCellDirect(column, row);
    return cell.Parent ?? cell;
  }

  public Cell GetCell(CellPosition position) => GetCell(position.X, position.Y);

  /// <summary>
  /// Merge cells by text range, if required.
  /// </summary>
  /// <param name="leftUpper">Like A3.</param>
  /// <param name="rightBottom">Like B8.</param>
  /// <returns>Merged cell.</returns>
  /// <exception cref="NotImplementedException"></exception>
  public Cell Merge(CellPosition leftUpper, CellPosition rightBottom)
  {
    if (leftUpper == rightBottom)
    {
      return GetCellDirect(leftUpper);
    }

    var leftCornerCell = GetCellDirect(Math.Min(leftUpper.X, rightBottom.X), Math.Max(rightBottom.Y, leftUpper.Y));

    var other = GetRangeDirect(leftUpper, rightBottom).Where(x => x != leftCornerCell);
    foreach (var cell in other)
    {
      if (cell.IsMerged)
      {
        cell.Break();
      }

      leftCornerCell.AddChild(cell);
    }

    return leftCornerCell;
  }

  public IEnumerable<Cell> GetRange(CellPosition leftUpper, CellPosition rightBottom)
    => GetRangeDirect(leftUpper, rightBottom).Where(x => x.Parent == null);

  private IEnumerable<Cell> GetRangeDirect(CellPosition leftUpper, CellPosition rightBottom)
  {
    if (leftUpper == rightBottom)
    {
      yield return GetCellDirect(leftUpper);
    }
    else
    {
      var x = Math.Min(leftUpper.X, rightBottom.X);
      var xMax = Math.Max(rightBottom.X, leftUpper.X);
      int y = Math.Min(leftUpper.Y, rightBottom.Y);
      var yMax = Math.Max(rightBottom.Y, leftUpper.Y);

      if (xMax > ColumnsCount || yMax > RowsCount)
      {
        throw new ArgumentOutOfRangeException(xMax > ColumnsCount ? nameof(xMax) : nameof(yMax));
      }

      for (; y < yMax; ++y)
      {
        for (; x < xMax; ++x)
        {
          yield return GetCellDirect(x, y);
        }
      }
    }
  }

  private Cell GetCellDirect(int column, int row) => cells[row * columns.Count + column];

  private Cell GetCellDirect(CellPosition position) => GetCellDirect(position.X, position.Y);
}