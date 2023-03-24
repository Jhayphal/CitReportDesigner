using System;
using ReactiveUI;

namespace ControlsSandbox.ViewModels;

public class NumericUpDownViewModel : ViewModelBase
{
  private int minValue;
  private int value;
  private int maxValue;
  private string text;

  /// <summary>
  /// For designer.
  /// </summary>
  public NumericUpDownViewModel()
  {
    minValue = 0;
    value = 10;
    maxValue = 100;
    text = value.ToString();
  }
  
  public NumericUpDownViewModel(int value, int minValue = 0, int maxValue = int.MaxValue)
  {
    this.minValue = minValue;
    this.value = value;
    this.maxValue = maxValue;
    text = this.value.ToString();
  }
  
  public int MinValue
  {
    get => minValue;
    set => this.RaiseAndSetIfChanged(ref minValue, value);
  }
  
  public int Value
  {
    get => value;
    set
    {
      if (value != this.value)
      {
        if (value >= MinValue && value <= MaxValue)
        {
          this.value = value;
          this.RaisePropertyChanged();
          text = this.value.ToString();
        }

        this.RaisePropertyChanged(nameof(Text));
      }
    }
  }
  
  public int MaxValue 
  {
    get => maxValue;
    set => this.RaiseAndSetIfChanged(ref maxValue, value);
  }

  public string Text
  {
    get => string.IsNullOrEmpty(text) 
      ? "0" 
      : text;
    set => Value = string.IsNullOrEmpty(value) 
      ? 0 
      : int.TryParse(value, out var newValue) 
        ? newValue 
        : this.value;
  }

  public void Up()
  {
    if (Value + 1 <= MaxValue)
    {
      ++Value;
    }
  }

  public void Down()
  {
    if (Value - 1 >= MinValue)
    {
      --Value;
    }
  }
}