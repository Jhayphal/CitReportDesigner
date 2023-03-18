using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlsSandbox.ViewModels
{
  public class CellViewModel : ViewModelBase, IBounds
  {
    public string Text { get; set; } = "Text for designer";

    public double Width { get; set; } = 50d;

    public double Height { get; set; } = 5d;

    public double X { get; set; } = 0d;

    public double Y { get; set; } = 0d;

    public ReportSizeUnit SizeUnit => ReportSizeUnit.Millimeter;
  }
}
