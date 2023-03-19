using CitReport;
using ReactiveUI;
using System.Collections.Generic;
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
    this.table.CellsSizeChanged += Table_CellsSizeChanged;
    Cells = new ObservableCollection<CellViewModel>(table.Select(x => new CellViewModel(x)));
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
      if (table.X != value)
      {
        table.X = value;
        this.RaisePropertyChanged(nameof(X));
      }
    }
  }

  public double Y
  {
    get => table.Y;
    set
    {
      if (table.Y != value)
      {
        table.Y = value;
        this.RaisePropertyChanged(nameof(Y));
      }
    }
  }

  public double Width
  {
    get => table.Width;
    set
    {
      if (table.Width != value)
      {
        table.Width = value;
        this.RaisePropertyChanged(nameof(Width));
      }
    }
  }

  public double Height
  {
    get => table.Height;
    set
    {
      if (table.Height != value)
      {
        table.Height = value;
        this.RaisePropertyChanged(nameof(Height));
      }
    }
  }

  public ReportSizeUnit SizeUnit => ReportSizeUnit.Millimeter;
}
