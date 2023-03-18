using System.Drawing;

namespace CitReport;

public class TextBlock
{
  public double X { get; set; }
  
  public double Y { get; set; }

  public string Text { get; set; }

  public double Width { get; set; }

  public double Height { get; set; }

  public HorizontalAlignment HorizontalAlignment { get; set; }

  public VerticalAlignment VerticalAlignment { get; set; }

  public Color ForegroundColor { get; set; } = Color.Black;

  public Color BackgroundColor { get; set; } = Color.White;
}