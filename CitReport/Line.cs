using System.Drawing;

namespace CitReport;

public class Line
{
  public string Alias { get; set; }

  public double X1 { get; set; }

  public double X2 { get; set; }

  public double Y1 { get; set; }

  public double Y2 { get; set; }

  public double Width { get; set; }

  public Color Color { get; set; }

  public double Length
  {
    get
    {
      var x = X2 - X1;
      var y = Y2 - Y1;

      return Math.Sqrt(x * x + y * y);
    }
  }
}