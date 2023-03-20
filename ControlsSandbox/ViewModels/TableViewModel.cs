using CitReport;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CitReport.Extensions;

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
    cell.Properties.DisplayValue.Add(string.Empty, "Text for designer");
    cell = table.GetCell(1, 0);
    cell.Properties.DisplayValue.Add(string.Empty, "Col 2 Row 1");
    cell = table.GetCell(0, 1);
    cell.Properties.DisplayValue.Add(string.Empty, "Col 1 Row 2");
    cell = table.GetCell(1, 1);
    cell.Properties.DisplayValue.Add(string.Empty, "Col 2 Row 2");

    Cells = new ObservableCollection<CellViewModel>(table.Select(x => new CellViewModel(x)));
  }

  public TableViewModel(Table table)
  {
    this.table = table;
    this.table.CellsSizeChanged += Table_CellsSizeChanged;
    this.table.TablePositionChanged += Table_TablePositionChanged;
    this.table.TableSizeChanged += Table_TableSizeChanged;
    Cells = new ObservableCollection<CellViewModel>(table.Select(x => new CellViewModel(x)));
  }

  private void Table_TablePositionChanged(object sender, System.EventArgs e)
  {
    this.RaisePropertyChanged(nameof(X));
    this.RaisePropertyChanged(nameof(Y));
  }

  private void Table_TableSizeChanged(object sender, System.EventArgs e)
  {
    this.RaisePropertyChanged(nameof(Width));
    this.RaisePropertyChanged(nameof(Height));
  }

  private void Table_CellsSizeChanged(object sender, IEnumerable<Cell> e)
  {
    foreach (var cell in Cells.Where(x => e.Any(c => x.Equals(c))))
    {
      cell.RaisePropertyChanged(nameof(CellViewModel.X));
      cell.RaisePropertyChanged(nameof(CellViewModel.Y));
      cell.RaisePropertyChanged(nameof(CellViewModel.Width));
      cell.RaisePropertyChanged(nameof(CellViewModel.Height));
    }
  }

  public ObservableCollection<CellViewModel> Cells { get; }

  public double X
  {
    get => table.X;
    set
    {
      if (!table.X.AreEqual(value))
      {
        table.X = value;
        this.RaisePropertyChanged();
      }
    }
  }

  public double Y
  {
    get => table.Y;
    set
    {
      if (!table.Y.AreEqual(value))
      {
        table.Y = value;
        this.RaisePropertyChanged();
      }
    }
  }

  public double Width
  {
    get => table.Width;
    set
    {
      if (!table.Width.AreEqual(value))
      {
        table.Width = value;
        this.RaisePropertyChanged();
      }
    }
  }

  public double Height
  {
    get => table.Height;
    set
    {
      if (!table.Height.AreEqual(value))
      {
        table.Height = value;
        this.RaisePropertyChanged();
      }
    }
  }

  public ReportSizeUnit SizeUnit => ReportSizeUnit.Millimeter;
}
