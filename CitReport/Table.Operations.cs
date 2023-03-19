namespace CitReport;

public sealed partial class Table
{
  /// <summary>
  /// Merge cells by text range, if required.
  /// </summary>
  /// <param name="leftUpper">Like A3.</param>
  /// <param name="rightBottom">Like B8.</param>
  /// <returns>Merged cell.</returns>
  public Cell Merge(CellPosition leftUpper, CellPosition rightBottom)
  {
    if (leftUpper == rightBottom)
    {
      return GetCellDirect(leftUpper);
    }

    var leftCornerCell = GetCellDirect(Math.Min(leftUpper.Column, rightBottom.Column), Math.Max(rightBottom.Row, leftUpper.Row));

    var other = GetRangeDirect(leftUpper, rightBottom);
    foreach (var cell in other.Where(x => x != leftCornerCell))
    {
      if (cell.IsMerged)
      {
        cell.Break();
      }

      leftCornerCell.AddChild(cell);
    }
    
    OnCellsSizeChanged(other);

    return leftCornerCell;
  }
}
