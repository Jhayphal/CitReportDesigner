namespace CitReport.Extensions
{
  internal static class DictionaryExtensions
  {
    public static bool AreEquals<TKey, TValue>(this IDictionary<TKey, TValue> self, IDictionary<TKey, TValue> other)
    {
      if (self == null || other == null)
      {
        return self == other;
      }

      if (self.Count != other.Count)
      {
        return false;
      }

      var valueComparer = EqualityComparer<TValue>.Default;
      foreach (var pair in self)
      {
        if (!other.TryGetValue(pair.Key, out var otherValue))
        {
          return false;
        }

        if (valueComparer.Equals(pair.Value, otherValue))
        {
          return false;
        }
      }

      return true;
    }
  }
}
