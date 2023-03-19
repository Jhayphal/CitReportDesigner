using Avalonia.Layout;
using Avalonia.Media;
using ReactiveUI;

namespace ControlsSandbox.ViewModels;

public class CellViewModel : ViewModelBase, IBounds
{
  private readonly CitReport.Cell cell;

  /// <summary>
  /// Конструктор для дизайнера.
  /// </summary>
  public CellViewModel()
  {
    var table = new CitReport.Table(
      new double[] { 0f, 50f, 100f }, 
      new double[] { 0f, 5f, 10f });
    
    cell = table.GetCell(0, 0);
    cell.Properties.DisplayValue.Add(string.Empty, "Text for designer");
  }

  public CellViewModel(CitReport.Cell cell)
  {
    this.cell = cell;
  }

  public string Text
  {
    get
    {
      cell.Properties.DisplayValue.TryGetValue(DesignerEnvironment.Current.Language, out string text);
      return text ?? string.Empty;
    }
    set
    {
      cell.Properties.DisplayValue.TryGetValue(DesignerEnvironment.Current.Language, out string text);
      text ??= string.Empty;
      if (text != value)
      {
        cell.Properties.DisplayValue[DesignerEnvironment.Current.Language] = value;
        this.RaisePropertyChanged(nameof(Text));
      }
    }
  }

  public double X
  {
    get => cell.X;
    set => cell.X = value;
  }

  public double Y
  {
    get => cell.Y;
    set => cell.Y = value;
  }

  public double Width
  {
    get => cell.Width;
    set => cell.Width = value;
  }

  public double Height
  {
    get => cell.Height;
    set => cell.Height = value;
  }

  public int Row => cell.Row;

  public int RowSpan => 1;

  public int Column => cell.Column;

  public int ColumnSpan => 1;

  public ReportSizeUnit SizeUnit => ReportSizeUnit.Millimeter;

  public HorizontalAlignment HorizontalAlignment => AlignmentConverter.ConvertHorizontal(cell.Style.HorizontalAlignment);

  public VerticalAlignment VerticalAlignment => AlignmentConverter.ConvertVertical(cell.Style.VerticalAlignment);

  public IBrush ForegroundColor { get; set; } = Brushes.Black;

  public IBrush BackgroundColor { get; set; } = Brushes.White;

  public bool Equals(CitReport.Cell cell) => cell == this.cell;

  public override bool Equals(object obj) => obj is CellViewModel viewModel && Equals(viewModel.cell);

  public override int GetHashCode() => cell.GetHashCode();

  public override string ToString() => cell.ToString();
}
