using System.Collections.ObjectModel;

namespace ControlsSandbox.ViewModels
{
  public class MainWindowViewModel : ViewModelBase
  {
    public ObservableCollection<BlockViewModel> Blocks { get; set; } = new()
    {
      new BlockViewModel { Type = "PH" },
      new BlockViewModel { Type = "RH" },
      new BlockViewModel { Type = "DT" },
      new BlockViewModel { Type = "RF" }
    };
  }
}
