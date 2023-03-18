using System.Collections.ObjectModel;

namespace ControlsSandbox.ViewModels
{
  public class TableViewModel : ViewModelBase, IBounds
  {
    public ObservableCollection<CellViewModel> Cells { get; set; }

    public double Width { get; set; } = 100d;

    public double Height { get; set; } = 20d;

    public double X { get; set; } = 20d;
    
    public double Y { get; set; } = 0d;

    public ReportSizeUnit SizeUnit => ReportSizeUnit.Millimeter;
  }
}
