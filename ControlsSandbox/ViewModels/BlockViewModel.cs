﻿using Avalonia.Media;
using CitReport;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlsSandbox.ViewModels;

public class BlockViewModel : ViewModelBase
{
  private static readonly Dictionary<string, string> descriptions = new()
  {
    { "RH", "Заголовок отчета" },
    { "PH", "Заголовок страницы" },
    { "IH", "Заголовок таблицы" },
    { "CH", "Начало прерывания" },
    { "DT", "Детальный блок" },
    { "CF", "Окончание прерывания" },
    { "PF", "Окончание страницы" },
    { "RF", "Окончание отчета" }
  };

  private readonly BodyBlock block;

  private bool isExpanded = true;
  private bool isSelected;

  public BlockViewModel()
  {
    var report = new Report();
    block = new BodyBlock(report, id: 1) { Code = "PH" };
    block.Options.Add(new BlkHOption { Height = 30 });
  }

  public BlockViewModel(BodyBlock block, int id)
  {
    this.block = block;
    Id = id;

    var textBlocks = block.TextBlocks.Select(x => (ViewModelBase)new TextBlockViewModel(x));
    var tables = block.Tables.Select(x => (ViewModelBase)new TableViewModel(x));
    Items = new ObservableCollection<ViewModelBase>(textBlocks.Concat(tables));
  }

  public string Type => block.Code;

  public int Id
  {
    get => block.Id;
    set => block.Id = value;
  }

  public ObservableCollection<ViewModelBase> Items { get; }

  public int ItemsCount => (block.Tables?.Count).GetValueOrDefault();

  public string Header => $"{Type} ({Id}) - {GetBlockTypeText()} [{GetItemsText()}]";

  public double HeaderFont => 3d;

  public double HeaderHeight => 5d;

  public double SummaryHeight => HeaderHeight + Height;

  public double Width { get; set; } = 190d;

  public double Height => block.Height;

  public string HeightMillimetersText => $"{block.Height} мм";

  public string HeaderArrowGlyph => IsExpanded ? "🠋" : "🠉";

  public bool IsExpanded
  {
    get => isExpanded;
    set
    {
      this.RaiseAndSetIfChanged(ref isExpanded, value);
      this.RaisePropertyChanged(nameof(HeaderArrowGlyph));
    }
  }

  public bool IsSelected
  {
    get => isSelected;
    set
    {
      this.RaiseAndSetIfChanged(ref isSelected, value);
      this.RaisePropertyChanged(nameof(BlockHeaderColorStart));
      this.RaisePropertyChanged(nameof(BlockHeaderColorStop));
    }
  }

  public Avalonia.Thickness HeaderItemsPaddings { get; } = new Avalonia.Thickness(1, 0);

  public Color BlockHeaderColorStart => IsSelected ? Color.FromUInt32(0xFFFCFCFD) : Color.FromUInt32(0xFFFEFEFE);
  
  public Color BlockHeaderColorStop => IsSelected ? Color.FromUInt32(0xFFCED3E7) : Color.FromUInt32(0xFFEFEFEF);

  public void ChangeSelectedState() => IsSelected = !IsSelected;

  public void ChangeExpandedState() => IsExpanded = !IsExpanded;

  private string GetItemsText() => ItemsCount switch
  {
    1 => $"{ItemsCount} элемент",
    2 or 3 or 4 => $"{ItemsCount} элемента",
    _ => $"{ItemsCount} элементов",
  };

  private string GetBlockTypeText()
    => descriptions.TryGetValue(Type.ToUpper(), out string description)
      ? description
      : "Неизвестный тип";
}
