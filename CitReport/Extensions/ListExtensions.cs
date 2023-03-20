namespace CitReport.Extensions
{
  internal static class ListExtensions
  {
    public static bool AreEquals<T>(this IList<T> self, IList<T> other)
    {
      if (self == null || other == null)
      {
        return self == other;
      }

      if (self.Count != other.Count)
      {
        return false;
      }

      var comparer = EqualityComparer<T>.Default;
      for (var i = 0; i < self.Count; ++i)
      {
        if (!comparer.Equals(self[i], other[i]))
        {
          return false;
        }
      }

      return true;
    }
  }
}
