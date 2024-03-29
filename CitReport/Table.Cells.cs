﻿namespace CitReport;

public sealed partial class Table
{
  public double GetCellX(Cell cell) => borders.Columns[cell.Column] - borders.X;

  public void SetCellX(Cell cell, double value)
  {
    value += borders.X;
    var column = cell.Column;
    if (borders.Columns[column] != value)
    {
      borders.Columns[column] = value;
      OnColumnXChanged(column);
      OnTableSizeChanged(this);
    }
  }

  public double GetCellY(Cell cell) => borders.Rows[cell.Row] - borders.Y;

  public void SetCellY(Cell cell, double value)
  {
    value += borders.Y;
    var row = cell.Row;
    if (borders.Rows[row] != value)
    {
      borders.Rows[row] = value;
      OnRowYChanged(row);
      OnTableSizeChanged(this);
    }
  }

  public Cell GetCell(int column, int row)
  {
    var cell = GetCellDirect(column, row);
    return cell.Parent ?? cell;
  }

  public Cell GetCell(CellPosition position) => GetCell(position.Column, position.Row);

  private Cell GetCellDirect(int column, int row) => cells[row * ColumnsCount + column];

  private Cell GetCellDirect(CellPosition position) => GetCellDirect(position.Column, position.Row);
}
