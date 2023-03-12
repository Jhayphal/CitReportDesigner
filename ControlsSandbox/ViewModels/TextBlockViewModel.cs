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
      get => MeasurementConverter.MillimetersToPixels(textBlock.X);
      set => textBlock.X = (float)MeasurementConverter.PixelsToMillimeters(value);
    }

    public double Y
    {
      get => MeasurementConverter.MillimetersToPixels(textBlock.Y);
      set => textBlock.Y = (float)MeasurementConverter.PixelsToMillimeters(value);
    }

    public string Text
    {
      get => textBlock.Text;
      set => textBlock.Text = value;
    }

    public double Width
    {
      get => MeasurementConverter.MillimetersToPixels(textBlock.Width);
      set => textBlock.Width = (float)MeasurementConverter.PixelsToMillimeters(value);
    }

    public double Height
    {
      get => MeasurementConverter.MillimetersToPixels(textBlock.Height);
      set => textBlock.Height = (float)MeasurementConverter.PixelsToMillimeters(value);
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
