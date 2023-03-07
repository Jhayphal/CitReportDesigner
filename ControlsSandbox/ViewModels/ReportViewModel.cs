using Avalonia.Controls;
using CitReport;
using System.Collections.Generic;
using System.Linq;

namespace ControlsSandbox.ViewModels
{
  public class ReportViewModel : ViewModelBase
  {
    private readonly Report report;

    public ReportViewModel(Report report)
    {
      this.report = report;
      Blocks = this.report.Blocks.Select((b, i) => new BlockViewModel(b, i)).ToList();
    }

    public double Width { get; set; } = MeasurementConverter.MillimetersToPixels(190);

    public double Height { get; set; } = MeasurementConverter.MillimetersToPixels(270);

    public List<BlockViewModel> Blocks { get; }
  }
}
