using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests;

public static class AssertExtensions
{
  public static void AreEquals<T>(this T[] expected, T[] actual)
  {
    if (expected == null || actual == null)
    {
      Assert.IsTrue(ReferenceEquals(expected, actual));

      return;
    }

    Assert.AreEqual(expected.Length, actual.Length);

    for (int i = 0; i < expected.Length; ++i)
    {
      Assert.AreEqual(expected[i], actual[i]);
    }
  }
}