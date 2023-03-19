namespace CitReport;

public class TableBorders
{
  public TableBorders(IEnumerable<double> columns, IEnumerable<double> rows)
  {
    Columns = new List<double>(columns);
    Rows = new List<double>(rows);
  }

  public readonly IList<double> Columns;
  public readonly IList<double> Rows;

  public double X
  {
    get => Columns[FirstColumn];
    set
    {
      var oldX = Columns[FirstColumn];
      var newX = value;

      if (oldX != newX)
      {
        var shift = newX - oldX;

        for (int i = ColumnsCount; i >= FirstColumn; --i)
        {
          Columns[i] += shift;
        }
      }
    }
  }

  public double Y
  {
    get => Rows[FirstRow];
    set
    {
      var oldY = Rows[FirstRow];
      var newY = value;

      if (oldY != newY)
      {
        var shift = newY - oldY;

        for (int i = RowsCount; i >= FirstRow; --i)
        {
          Rows[i] += shift;
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

  public int ColumnsCount => Columns.Count - 1;

  public const int FirstColumn = 0;

  public int LastColumn => ColumnsCount - 1;

  public int RowsCount => Rows.Count - 1;

  public const int FirstRow = 0;

  public int LastRow => RowsCount - 1;

  public double GetColumnWidth(int index) => Columns[index + 1] - Columns[index];

  public void SetColumnWidth(int index, double newWidth)
  {
    var oldWidth = GetColumnWidth(index);
    var shift = newWidth - oldWidth;
    Columns[index + 1] += shift;
  }

  public double GetRowHeight(int index) => Rows[index + 1] - Rows[index];

  public void SetRowHeight(int index, double newHeight)
  {
    var oldHeight = GetRowHeight(index);
    var shift = newHeight - oldHeight;
    Rows[index + 1] += shift;
  }

  private double GetWidth() => Columns[ColumnsCount] - Columns[FirstColumn];

  private void SetWidth(double newWidth)
  {
    var oldWidth = GetWidth();
    var scale = newWidth / oldWidth;

    for (int i = LastColumn; i >= FirstColumn; --i)
    {
      SetColumnWidth(i, GetColumnWidth(i) * scale);
    }
  }

  private double GetHeight() => Rows[RowsCount] - Rows[FirstRow];

  private void SetHeight(double newHeight)
  {
    var oldHeight = GetHeight();
    var scale = newHeight / oldHeight;

    for (int i = RowsCount; i >= FirstRow; --i)
    {
      SetRowHeight(i, GetRowHeight(i) * scale);
    }
  }
}