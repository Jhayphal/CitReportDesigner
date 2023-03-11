using Avalonia.Layout;
using Avalonia.Media;
using System;

namespace ControlsSandbox.ViewModels
{
  public class TextBlockViewModel : ViewModelBase
  {
    private readonly CitReport.TextBlock textBlock;

    public TextBlockViewModel(CitReport.TextBlock textBlock)
    {
      this.textBlock = textBlock;
    }

    public double X
    {
      get => textBlock.X;
      set => textBlock.X = (float)value;
    }

    public float Y
    {
      get => textBlock.Y;
      set => textBlock.Y = (float)value;
    }

    public string Text
    {
      get => textBlock.Text;
      set => textBlock.Text= value;
    }

    public float Width
    {
      get => textBlock.Width;
      set => textBlock.Width = (float)value;
    }

    public float Height
    {
      get => textBlock.Height;
      set => textBlock.Height = (float)value;
    }

    public HorizontalAlignment HorizontalAlignment => ConvertHAligment(textBlock.HorizontalAlignment);

    public VerticalAlignment VerticalAlignment => ConvertVAligment(textBlock.VerticalAlignment);

    public IBrush ForegroundColor { get; set; } = Brushes.Black;

    public IBrush BackgroundColor { get; set; } = Brushes.White;

    private HorizontalAlignment ConvertHAligment(CitReport.HorizontalAlignment alignment) => alignment switch
    {
      CitReport.HorizontalAlignment.Center => HorizontalAlignment.Center,
      CitReport.HorizontalAlignment.Right => HorizontalAlignment.Right,
      CitReport.HorizontalAlignment.Left => HorizontalAlignment.Left,
      _ => throw new InvalidOperationException(alignment.ToString())
    };

    private VerticalAlignment ConvertVAligment(CitReport.VerticalAlignment alignment) => alignment switch
    {
      CitReport.VerticalAlignment.Center => VerticalAlignment.Center,
      CitReport.VerticalAlignment.Top => VerticalAlignment.Top,
      CitReport.VerticalAlignment.Bottom => VerticalAlignment.Bottom,
      _ => throw new InvalidOperationException(alignment.ToString())
    };
  }
}
