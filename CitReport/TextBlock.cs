using System.Drawing;

namespace CitReport;

public class TextBlock
{
  public float X { get; set; }
  
  public float Y { get; set; }

  public string Text { get; set; }

  public float Width { get; set; }

  public float Height { get; set; }

  public HorizontalAlignment HorizontalAlignment { get; set; }

  public VerticalAlignment VerticalAlignment { get; set; }

  public Color ForegroundColor { get; set; } = Color.Black;

  public Color BackgroundColor { get; set; } = Color.White;
}