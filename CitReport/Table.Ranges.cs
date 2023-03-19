namespace CitReport;

public sealed partial class Table
{
  public IEnumerable<Cell> GetRange(CellPosition leftUpper, CellPosition rightBottom)
  {
    var cells = new HashSet<Cell>();
    foreach (var cell in GetRangeDirect(leftUpper, rightBottom))
    {
      cells.Add(cell.Parent ?? cell);
    }
    return cells;
  }

  private IEnumerable<Cell> GetRangeDirect(CellPosition leftUpper, CellPosition rightBottom)
  {
    if (leftUpper == rightBottom)
    {
      yield return GetCellDirect(leftUpper);
    }
    else
    {
      var x = Math.Min(leftUpper.Column, rightBottom.Column);
      var xMax = Math.Max(rightBottom.Column, leftUpper.Column);
      int y = Math.Min(leftUpper.Row, rightBottom.Row);
      var yMax = Math.Max(rightBottom.Row, leftUpper.Row);

      if (xMax > borders.LastColumn || yMax > borders.LastRow)
      {
        throw new ArgumentOutOfRangeException(xMax > borders.LastColumn ? nameof(xMax) : nameof(yMax));
      }

      for (; y <= yMax; ++y)
      {
        for (; x <= xMax; ++x)
        {
          yield return GetCellDirect(x, y);
        }

        x = Math.Min(leftUpper.Column, rightBottom.Column);
      }
    }
  }
}
