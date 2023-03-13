using Avalonia.Layout;
using Avalonia.Media;
using ReactiveUI;
using System;

namespace ControlsSandbox.ViewModels
{
  public class TextBlockViewModel : ViewModelBase, IControlBounds
  {
    private readonly CitReport.TextBlock textBlock;

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
          textBlock.X = (float)value;
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
          textBlock.Y = (float)value;
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
          textBlock.Width = (float)value;
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
          textBlock.Height = (float)value;
          this.RaisePropertyChanged(nameof(Height));
        }
      }
    }

    public ReportSizeUnit SizeUnit => ReportSizeUnit.Millimeter;

    public HorizontalAlignment HorizontalAlignment => ConvertHAlignment(textBlock.HorizontalAlignment);

    public VerticalAlignment VerticalAlignment => ConvertVAlignment(textBlock.VerticalAlignment);

    public IBrush ForegroundColor { get; set; } = Brushes.Black;

    public IBrush BackgroundColor { get; set; } = Brushes.White;

    private HorizontalAlignment ConvertHAlignment(CitReport.HorizontalAlignment alignment) => alignment switch
    {
      CitReport.HorizontalAlignment.Center => HorizontalAlignment.Center,
      CitReport.HorizontalAlignment.Right => HorizontalAlignment.Right,
      CitReport.HorizontalAlignment.Left => HorizontalAlignment.Left,
      _ => throw new InvalidOperationException(alignment.ToString())
    };

    private VerticalAlignment ConvertVAlignment(CitReport.VerticalAlignment alignment) => alignment switch
    {
      CitReport.VerticalAlignment.Center => VerticalAlignment.Center,
      CitReport.VerticalAlignment.Top => VerticalAlignment.Top,
      CitReport.VerticalAlignment.Bottom => VerticalAlignment.Bottom,
      _ => throw new InvalidOperationException(alignment.ToString())
    };
  }
}
