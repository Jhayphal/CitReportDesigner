using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests
{
  public static class AssertExtensions
  {
    public static void AreEquals<T>(this T[] values, T[] other)
    {
      if (values == null || other == null)
      {
        Assert.IsTrue(ReferenceEquals(values, other));

        return;
      }

      Assert.AreEqual(values.Length, other.Length);

      for (int i = 0; i < values.Length; ++i)
      {
        Assert.AreEqual(values[i], other[i]);
      }
    }
  }
}