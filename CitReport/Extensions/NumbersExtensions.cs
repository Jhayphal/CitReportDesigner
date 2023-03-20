using System;

namespace CitReport.Extensions;

public static class NumbersExtensions
{
  public static bool AreEqual(this double value, double other) => Math.Abs(value - other) < 0.00001d;
}