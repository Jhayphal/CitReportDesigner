namespace CitReport.IO.Parser;

public static class DisplayFieldsHelper
{
  public static int GetCount(string text)
  {
    if (text == null)
    {
      return 0;
    }

    var count = 0;
    var position = 0;

    while (position < text.Length)
    {
      while (position < text.Length && text[position] != '_')
      {
        ++position;
      }

      ++count;

      if (position > 0 && text[position - 1] == '<'
        && position + 1 < text.Length && text[position + 1] == '>') // field like <_>
      {
        position += 2;
        continue;
      }

      while (position < text.Length && text[position] == '_') // field like ____
      {
        ++position;
      }

      if (position < text.Length && text[position] == '.') // field like ______.__
      {
        ++position;
      }

      while (position < text.Length && text[position] == '_')
      {
        ++position;
      }
    }

    return count;
  }
}