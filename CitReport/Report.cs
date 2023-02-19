using System.Drawing;

namespace CitReport
{
  public class Report
  {
    public IEnumerable<Metadata> Metadata { get; set; }

    public IEnumerable<Expression> CodeBehind { get; set; }

    public ReportBlock ReportBlock { get; set; }

    public IEnumerable<BodyBlock> BodyBlocks { get; set; }
  }

  public abstract class Block
  {
    public string Code { get; set; }

    public IEnumerable<Option> Options { get; set; }
  }

  public class ReportBlock : Block
  {
    public IEnumerable<Expression> AfterStartActions { get; set; }

    public IEnumerable<Expression> AfterEndActions { get; set; }

    public IEnumerable<Expression> DoActions { get; set; }
  }

  public class BodyBlock : Block
  {
    public IEnumerable<Metadata> Metadata { get; set; }

    public Fonts Fonts { get; set; }

    public float Height => (Options?.OfType<BlockHeightOption>().FirstOrDefault()?.Height).GetValueOrDefault();
  }

  public class Metadata
  {
    public string Value { get; set; }
  }

  [Flags]
  public enum FontStyle
  {
    Regular = 0,
    Bold = 1,
    Italic = 2,
    Underline = 4
  }

  public class Fonts : List<FontInfo>
  {
    public IEnumerable<Metadata> Metadata { get; set; }
  }

  public class FontInfo
  {
    public int Id { get; set; }

    public string Family { get; set; }

    public float Size { get; set; }

    public FontStyle Style { get; set; }
  }

  public class Option
  {
    public virtual string Value { get; set; }
  }

  public class BlockHeightOption : Option
  {
    public override string Value
    {
      get => Height.ToString();
      set => Height = float.Parse(value);
    }

    public float Height { get; set; }
  }

  public class Expression
  {
    public string Value { get; set; }
  }

  public class Line
  {
    public string Alias { get; set; }

    public float X1 { get; set; }

    public float X2 { get; set; }

    public float Y1 { get; set; }

    public float Y2 { get; set; }

    public float Width { get; set; }

    public Color Color { get; set; }

    public float Length
    {
      get
      {
        var x = X2 - X1;
        var y = Y2 - Y1;

        return (float)Math.Sqrt(x * x + y * y);
      }
    }
  }

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
  }

  public class Cell
  {
    protected readonly List<Cell> children = new();

    public Cell(Table table)
    {
      Table = table;
    }

    public Table Table { get; }
    
    public Cell Parent { get; protected set; }

    public IEnumerable<Cell> Children => children;

    public Dictionary<string, string> DisplayValue { get; } = new Dictionary<string, string>();

    public Dictionary<string, string> Value { get; } = new Dictionary<string, string>();

    public int Column => Table.GetCellColumnIndex(this);

    public int Row => Table.GetCellColumnIndex(this);

    public float Width => Table.GetColumnWidth(Column);

    public float Height => Table.GetRowHeight(Row);

    public void AddChild(Cell cell)
    {
      children.Add(cell);
      cell.Parent = this;
    }

    public void SetAsParent(Cell cell)
    {
      cell.children.Add(this);
      Parent = cell;
    }
  }
}