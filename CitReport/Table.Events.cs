namespace CitReport;

public sealed partial class Table
{
  public event EventHandler TablePositionChanged;

  public event EventHandler TableSizeChanged;

  public event EventHandler<IEnumerable<Cell>> CellsSizeChanged;

  private void OnTablePositionChanged() => TablePositionChanged?.Invoke(this, EventArgs.Empty);

  private void OnTableSizeChanged(IEnumerable<Cell> cells)
  {
    if (cells is not null)
    {
      CellsSizeChanged?.Invoke(this, cells);
    }

    TableSizeChanged?.Invoke(this, EventArgs.Empty);
  }

  private void OnColumnWidthChanged(int index)
  {
    var notifyFrom = new CellPosition(index, 0);
    var notifyTo = new CellPosition(Math.Min(index + 1, borders.LastColumn), borders.LastRow);
    var cells = GetRange(notifyFrom, notifyTo);

    if (index == borders.LastColumn)
    {
      OnTableSizeChanged(cells);
    }
    else
    {
      CellsSizeChanged?.Invoke(this, cells);
    }
  }

  private void OnRowHeightChanged(int row)
  {
    var notifyFrom = new CellPosition(0, row);
    var notifyTo = new CellPosition(borders.LastColumn, Math.Min(row + 1, borders.LastRow));
    var cells = GetRange(notifyFrom, notifyTo);

    if (row == borders.LastRow)
    {
      OnTableSizeChanged(cells);
    }
    else
    {
      CellsSizeChanged?.Invoke(this, cells);
    }
  }

  private void OnCellsSizeChanged(IEnumerable<Cell> cells) => CellsSizeChanged?.Invoke(this, cells);

  private void OnColumnXChanged(int index)
  {
    if (index == 0)
    {
      OnTablePositionChanged();
    }
    else
    {
      var notifyFrom = new CellPosition(index, 0);
      var notifyTo = new CellPosition(Math.Min(index + 1, borders.LastColumn), borders.LastRow);
      var cells = GetRange(notifyFrom, notifyTo);

      CellsSizeChanged?.Invoke(this, cells);
    }
  }

  private void OnRowYChanged(int index)
  {
    if (index == 0)
    {
      OnTablePositionChanged();
    }
    else
    {
      var notifyFrom = new CellPosition(0, index);
      var notifyTo = new CellPosition(borders.LastColumn, Math.Min(index + 1, borders.LastRow));
      var cells = GetRange(notifyFrom, notifyTo);

      CellsSizeChanged?.Invoke(this, cells);
    }
  }
}
