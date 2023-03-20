using System.Collections;
using CitReport.Extensions;

namespace CitReport;

public sealed partial class Table : IEnumerable<Cell>
{
  private readonly IList<Cell> cells;
  private readonly TableBorders borders;

  public Table(IEnumerable<double> columns, IEnumerable<double> rows)
  {
    borders = new TableBorders(columns, rows);
    cells = new List<Cell>();

    var count = ColumnsCount * RowsCount;
    while (count-- > 0)
    {
      cells.Add(new Cell(this));
    }
  }

  public double X
  {
    get => borders.X;
    set
    {
      if (borders.X != value)
      {
        borders.X = value;
        OnTablePositionChanged();
      }
    }
  }

  public double Y
  {
    get => borders.Y;
    set
    {
      if (borders.Y != value)
      {
        borders.Y = value;
        OnTablePositionChanged();
      }
    }
  }

  public double Width
  {
    get => borders.Width;
    set
    {
      if (borders.Width != value)
      {
        borders.Width = value;
        OnTableSizeChanged(this);
      }
    }
  }

  public double Height
  {
    get => borders.Height;
    set
    {
      if (borders.Height != value)
      {
        borders.Height = value;
        OnTableSizeChanged(this);
      }
    }
  }

  public int ColumnsCount => borders.ColumnsCount;

  public int RowsCount => borders.RowsCount;

  public IEnumerator<Cell> GetEnumerator()
    => GetRange(
      new CellPosition(0, 0), 
      new CellPosition(borders.LastColumn, borders.LastRow)).GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

  public int GetColumnIndex(Cell cell)
  {
    var index = cells.IndexOf(cell);
    if (index == -1)
    {
      return index;
    }

    return index % ColumnsCount;
  }

  public int GetRowIndex(Cell cell)
  {
    var index = cells.IndexOf(cell);
    if (index == -1)
    {
      return index;
    }
    
    return index / ColumnsCount;
  }

  public double GetColumnWidth(int index) => borders.GetColumnWidth(index);

  public void SetColumnWidth(int column, double width)
  {
    if (!width.AreEqual(GetColumnWidth(column)))
    {
      borders.SetColumnWidth(column, width);
      OnColumnWidthChanged(column);
    }
  }

  public double GetRowHeight(int index) => borders.GetRowHeight(index);

  public void SetRowHeight(int row, double height)
  {
    if (!height.AreEqual(GetRowHeight(row)))
    {
      borders.SetRowHeight(row, height);
      OnRowHeightChanged(row);
    }
  }
}