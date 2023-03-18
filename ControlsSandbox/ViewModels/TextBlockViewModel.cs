using Avalonia.Layout;
using Avalonia.Media;
using ReactiveUI;

namespace ControlsSandbox.ViewModels;

public class TextBlockViewModel : ViewModelBase, IBounds
{
  private readonly CitReport.TextBlock textBlock;

  /// <summary>
  /// Конструктор для дизайнера.
  /// </summary>
  public TextBlockViewModel()
  {
    textBlock = new CitReport.TextBlock
    {
      Text = "Text for designer",
      X = 10f,
      Y = 10f,
      Width = 40f,
      Height = 5f,
      HorizontalAlignment = CitReport.HorizontalAlignment.Center,
      VerticalAlignment = CitReport.VerticalAlignment.Center
    };
  }

  public TextBlockViewModel(CitReport.TextBlock textBlock)
  {
    this.textBlock = textBlock;
  }

  public double X
  {
    get => textBlock.X;
    set
    {
      if (textBlock.X != value)
      {
        textBlock.X = value;
        this.RaisePropertyChanged(nameof(X));
      }
    }
  }

  public double Y
  {
    get => textBlock.Y;
    set
    {
      if (textBlock.Y != value)
      {
        textBlock.Y = value;
        this.RaisePropertyChanged(nameof(Y));
      }
    }
  }

  public string Text
  {
    get => textBlock.Text;
    set
    {
      if (textBlock.Text != value)
      {
        textBlock.Text = value;
        this.RaisePropertyChanged(nameof(Text));
      }
    }
  }

  public double Width
  {
    get => textBlock.Width;
    set
    {
      if (textBlock.Width != value)
      {
        textBlock.Width = value;
        this.RaisePropertyChanged(nameof(Width));
      }
    }
  }

  public double Height
  {
    get => textBlock.Height;
    set
    {
      if (textBlock.Height != value)
      {
        textBlock.Height = value;
        this.RaisePropertyChanged(nameof(Height));
      }
    }
  }

  public ReportSizeUnit SizeUnit => ReportSizeUnit.Millimeter;

  public HorizontalAlignment HorizontalAlignment => AlignmentConverter.ConvertHorizontal(textBlock.HorizontalAlignment);

  public VerticalAlignment VerticalAlignment => AlignmentConverter.ConvertVertical(textBlock.VerticalAlignment);

  public IBrush ForegroundColor { get; set; } = Brushes.Black;

  public IBrush BackgroundColor { get; set; } = Brushes.White;
}
