using System.Drawing;

namespace CitReport;

public class CellStyle
{
  public FontInfo Font { get; set; }

  public Color ForegroundColor { get; set; } = Color.Black;

  public Color BackgroundColor { get; set; } = Color.Gray;

  public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Center;

  public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Center;
}