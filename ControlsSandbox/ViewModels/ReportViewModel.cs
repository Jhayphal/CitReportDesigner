using CitReport;
using System.Collections.Generic;
using System.Linq;

namespace ControlsSandbox.ViewModels;

public class ReportViewModel : ViewModelBase
{
  private readonly Report report;

  public ReportViewModel(Report report)
  {
    this.report = report;
    Blocks = this.report.Blocks.Select((b, i) => 
    {
      var vm = new BlockViewModel(b, i);
      vm.PropertyChanged += BlockViewModel_PropertyChanged;
      return vm;
    }).ToList();
  }

  private void BlockViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (e.PropertyName == nameof(BlockViewModel.IsSelected) && sender is BlockViewModel vm && vm.IsSelected)
    {
      foreach(var block in Blocks)
      {
        if (block.IsSelected && !ReferenceEquals(block, vm))
        {
          block.IsSelected = false;
        }
      }
    }
  }

  public double Width { get; set; } = 190f;

  public double Height { get; set; } = 270f;

  public List<BlockViewModel> Blocks { get; }
}
