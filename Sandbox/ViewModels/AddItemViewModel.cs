﻿using ReactiveUI;
using Sandbox.Models;
using System.Reactive;

namespace Sandbox.ViewModels
{
  public class AddItemViewModel : ViewModelBase
  {
    private string description;

    public AddItemViewModel()
    {
      var okEnabled = this.WhenAnyValue(
        x => x.Description,
        x => !string.IsNullOrWhiteSpace(x));

      Ok = ReactiveCommand.Create(() => new TodoItem { Description = Description }, okEnabled);
      Cancel = ReactiveCommand.Create(() => { });
    }

    public string Description 
    { 
      get => description; 
      set => this.RaiseAndSetIfChanged(ref description, value);
    }

    public ReactiveCommand<Unit, TodoItem> Ok { get; }

    public ReactiveCommand<Unit, Unit> Cancel { get; }
  }
}
