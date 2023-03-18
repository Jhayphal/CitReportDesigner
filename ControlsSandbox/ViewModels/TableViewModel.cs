using Avalonia.Controls;
using CitReport;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlsSandbox.ViewModels;

public class TableViewModel : ViewModelBase, IBounds
{
  private readonly Table table;

  /// <summary>
  /// Конструктор для дизайнера.
  /// </summary>
  public TableViewModel()
  {
    table = new Table(new double[] { 0f, 50f, 100f }, new double[] { 0f, 5f, 10f });
    var cell = table.GetCell(0, 0);
    cell.DisplayValue.Add(string.Empty, "Text for designer");
    cell = table.GetCell(1, 0);
    cell.DisplayValue.Add(string.Empty, "Col 2 Row 1");
    cell = table.GetCell(0, 1);
    cell.DisplayValue.Add(string.Empty, "Col 1 Row 2");
    cell = table.GetCell(1, 1);
    cell.DisplayValue.Add(string.Empty, "Col 2 Row 2");

    Cells = new ObservableCollection<CellViewModel>(table.Select(x => new CellViewModel(x)));
  }

  public TableViewModel(Table table)
  {
    this.table = table;

    Cells = new ObservableCollection<CellViewModel>(table.Select(x => new CellViewModel(x)));

    ColumnDefinitions = new ColumnDefinitions();
    ColumnDefinitions.AddRange(
      Enumerable.Range(0, this.table.ColumnsCount)
      .Select(x => new ColumnDefinition(GridLength.Auto)));

    RowDefinitions = new RowDefinitions();
    RowDefinitions.AddRange(
      Enumerable.Range(0, this.table.ColumnsCount)
      .Select(x => new RowDefinition(GridLength.Auto)));
  }

  public ObservableCollection<CellViewModel> Cells { get; }

  public double X
  {
    get => table.X;
    set => table.X = value;
  }

  public double Y
  {
    get => table.Y;
    set => table.Y = value;
  }

  public double Width
  {
    get => table.Width;
    set => table.Width = value;
  }

  public double Height
  {
    get => table.Height;
    set => table.Height = value;
  }

  public ReportSizeUnit SizeUnit => ReportSizeUnit.Millimeter;

  public ColumnDefinitions ColumnDefinitions { get; }

  public RowDefinitions RowDefinitions { get; }
}
