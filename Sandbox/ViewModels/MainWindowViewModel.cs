using Sandbox.Services;

namespace Sandbox.ViewModels
{
  public class MainWindowViewModel : ViewModelBase
  {
    public string Greeting => "Welcome to Avalonia!";

    public MainWindowViewModel(Database db)
    {
      List = new TodoListViewModel(db.GetItems());
    }

    public TodoListViewModel List { get; }
  }
}
