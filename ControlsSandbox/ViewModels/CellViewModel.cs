using Avalonia.Layout;
using Avalonia.Media;
using ReactiveUI;
using System;

namespace ControlsSandbox.ViewModels;

public class CellViewModel : ViewModelBase, IBounds
{
  private readonly CitReport.Cell cell;

  public event EventHandler<string> OnNeedTableRedraw;

  /// <summary>
  /// Конструктор для дизайнера.
  /// </summary>
  public CellViewModel()
  {
    var table = new CitReport.Table(
      new double[] { 0f, 50f, 100f }, 
      new double[] { 0f, 5f, 10f });
    
    cell = table.GetCell(0, 0);
    cell.DisplayValue.Add(string.Empty, "Text for designer");
  }

  public CellViewModel(CitReport.Cell cell)
  {
    this.cell = cell;
  }

  public string Text
  {
    get
    {
      cell.DisplayValue.TryGetValue(DesignerEnvironment.Current.Language, out string text);
      return text ?? string.Empty;
    }
    set
    {
      cell.DisplayValue.TryGetValue(DesignerEnvironment.Current.Language, out string text);
      text ??= string.Empty;
      if (text != value)
      {
        cell.DisplayValue[DesignerEnvironment.Current.Language] = value;
        this.RaisePropertyChanged(nameof(Text));
      }
    }
  }

  public double X
  {
    get => cell.Table.Columns[cell.Column];
    set
    {
      var column = cell.Column;
      if (cell.Table.Columns[column] != value)
      {
        cell.Table.Columns[column] = value;

        OnNeedTableRedraw.Invoke(this, nameof(X));
        OnNeedTableRedraw.Invoke(this, nameof(Width));
      }
    }
  }

  public double Y
  {
    get => cell.Table.Rows[cell.Row];
    set
    {
      var row = cell.Row;
      if (cell.Table.Rows[row] != value)
      {
        cell.Table.Rows[row] = value;

        OnNeedTableRedraw.Invoke(this, nameof(Y));
        OnNeedTableRedraw.Invoke(this, nameof(Height));
      }
    }
  }

  public double Width
  {
    get => cell.Width;
    set
    {
      if (cell.Width != value)
      {
        cell.Width = value;

        OnNeedTableRedraw.Invoke(this, nameof(Width));
        OnNeedTableRedraw.Invoke(this, nameof(X));
      }
    }
  }

  public double Height
  {
    get => cell.Height;
    set
    {
      if (cell.Height != value)
      {
        cell.Height = value;

        OnNeedTableRedraw.Invoke(this, nameof(Height));
        OnNeedTableRedraw.Invoke(this, nameof(Y));
      }
    }
  }

  public int Row => cell.Row;

  public int RowSpan => 1;

  public int Column => cell.Column;

  public int ColumnSpan => 1;

  public ReportSizeUnit SizeUnit => ReportSizeUnit.Millimeter;

  public HorizontalAlignment HorizontalAlignment => AlignmentConverter.ConvertHorizontal(cell.HorizontalAlignment);

  public VerticalAlignment VerticalAlignment => AlignmentConverter.ConvertVertical(cell.VerticalAlignment);

  public IBrush ForegroundColor { get; set; } = Brushes.Black;

  public IBrush BackgroundColor { get; set; } = Brushes.White;

  public override bool Equals(object obj) => obj is CellViewModel viewModel && viewModel.cell.Equals(cell);

  public override int GetHashCode() => cell.GetHashCode();

  public override string ToString() => cell.ToString();
}
