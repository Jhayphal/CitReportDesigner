using Avalonia.Layout;
using System;

namespace ControlsSandbox;

public static class AlignmentConverter
{
  public static HorizontalAlignment ConvertHorizontal(CitReport.HorizontalAlignment alignment) => alignment switch
  {
    CitReport.HorizontalAlignment.Center => HorizontalAlignment.Center,
    CitReport.HorizontalAlignment.Right => HorizontalAlignment.Right,
    CitReport.HorizontalAlignment.Left => HorizontalAlignment.Left,
    _ => throw new InvalidOperationException(alignment.ToString())
  };

  public static VerticalAlignment ConvertVertical(CitReport.VerticalAlignment alignment) => alignment switch
  {
    CitReport.VerticalAlignment.Center => VerticalAlignment.Center,
    CitReport.VerticalAlignment.Top => VerticalAlignment.Top,
    CitReport.VerticalAlignment.Bottom => VerticalAlignment.Bottom,
    _ => throw new InvalidOperationException(alignment.ToString())
  };
}
