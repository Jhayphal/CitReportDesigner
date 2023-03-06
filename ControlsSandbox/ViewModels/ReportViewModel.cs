using CitReport;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public float Width { get; set; } = MillimetersToPixels(190);

    public float Height { get; set; } = MillimetersToPixels(270);

    public List<BlockViewModel> Blocks { get; }
  }
}
