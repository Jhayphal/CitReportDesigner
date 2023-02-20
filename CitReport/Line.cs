using System.Drawing;

namespace CitReport
{
  public class Line
  {
    public string Alias { get; set; }

    public float X1 { get; set; }

    public float X2 { get; set; }

    public float Y1 { get; set; }

    public float Y2 { get; set; }

    public float Width { get; set; }

    public Color Color { get; set; }

    public float Length
    {
      get
      {
        var x = X2 - X1;
        var y = Y2 - Y1;

        return (float)Math.Sqrt(x * x + y * y);
      }
    }
  }
}