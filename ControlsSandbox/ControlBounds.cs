﻿namespace ControlsSandbox;

public struct ControlBounds : IBounds
{
  public ControlBounds(IBounds bounds)
  {
    X = bounds.X;
    Y = bounds.Y;
    Width = bounds.Width;
    Height = bounds.Height;
    SizeUnit = bounds.SizeUnit;
  }

  public double X { get; set; }

  public double Y { get; set; }

  public double Width { get; set; }

  public double Height { get; set; }

  public ReportSizeUnit SizeUnit { get; set; }

  public override string ToString() => $"{X:###.#}:{Y:###.#} {Width:###.#}:{Height:###.#}";
}