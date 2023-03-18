using System.Collections;

namespace CitReport;

public sealed class Table : IEnumerable<Cell>
{
  private readonly List<Cell> cells;
  private readonly List<double> columns;
  private readonly List<double> rows;

  public Table(IEnumerable<double> columns, IEnumerable<double> rows)
  {
    this.columns = new List<double>(columns);
    this.rows = new List<double>(rows);

    cells = new List<Cell>();

    var count = ColumnsCount * RowsCount;
    while (count-- > 0)
    {
      cells.Add(new Cell(this));
    }
  }

  public double X
  {
    get => columns[0];
    set
    {
      var oldX = columns[0];
      var newX = value;

      if (oldX != newX)
      {
        var shift = newX - oldX;

        for (int i = ColumnsCount; i >= 0; --i)
        {
          columns[i] += shift;
        }
      }
    }
  }

  public double Y
  {
    get => rows[0];
    set
    {
      var oldY = rows[0];
      var newY = value;

      if (oldY != newY)
      {
        var shift = newY - oldY;

        for (int i = RowsCount; i >= 0; --i)
        {
          rows[i] += shift;
        }
      }
    }
  }

  public double Width
  {
    get => GetWidth();
    set => SetWidth(value);
  }

  public double Height
  {
    get => GetHeight();
    set => SetHeight(value);
  }

  public IEnumerable<double> Columns => columns;
  
  public int ColumnsCount => columns.Count - 1;

  public IEnumerable<double> Rows => rows;

  public int RowsCount => rows.Count - 1;

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
    
    return index / ColumnsCount;
  }

  public int GetCellColumnIndex(Cell cell)
  {
    var index = cells.IndexOf(cell);
    if (index == -1)
    {
      return index;
    }

    return index % ColumnsCount;
  }

  public double GetColumnWidth(int index) => columns[index + 1] - columns[index];

  public void SetColumnWidth(int index, double newWidth)
  {
    var oldWidth = GetColumnWidth(index);
    var shift = newWidth - oldWidth;
    columns[index + 1] += shift;
  }

  public double GetRowHeight(int index) => rows[index + 1] - rows[index];

  public void SetRowHeight(int index, double newHeight)
  {
    var oldHeight = GetRowHeight(index);
    var shift = newHeight - oldHeight;
    rows[index + 1] += shift;
  }

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

        x = Math.Min(leftUpper.X, rightBottom.X);
      }
    }
  }

  private Cell GetCellDirect(int column, int row) => cells[row * ColumnsCount + column];

  private Cell GetCellDirect(CellPosition position) => GetCellDirect(position.X, position.Y);

  private double GetWidth() => columns[ColumnsCount] - columns[0];

  private void SetWidth(double newWidth)
  {
    var oldWidth = GetWidth();
    var scale = newWidth / oldWidth;

    for (int i = ColumnsCount; i >= 0; --i)
    {
      SetColumnWidth(i, GetColumnWidth(i) * scale);
    }
  }

  private double GetHeight() => rows[RowsCount] - rows[0];

  private void SetHeight(double newHeight)
  {
    var oldHeight = GetHeight();
    var scale = newHeight / oldHeight;

    for (int i = RowsCount; i >= 0; --i)
    {
      SetRowHeight(i, GetRowHeight(i) * scale);
    }
  }
}