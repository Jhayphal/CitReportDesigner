using System.Text;

namespace CitReport.Services.Parser
{
  internal class Tokenizer
  {
    private readonly string text;
    private readonly StringBuilder builder = new(); 
    private int position;

    public Tokenizer(string text)
    {
      this.text = text;
    }

    public IEnumerable<string> GetTokens()
    {
      while (position < text.Length)
      {
        while (position < text.Length && char.IsWhiteSpace(text[position]))
        {
          ++position;
        }

        if (position >= text.Length)
        {
          yield break;
        }

        while (position < text.Length && text[position] != '(')
        {
          builder.Append(text[position]);
        }

        var token = builder.ToString();
        builder.Clear();

        yield return token;

        int brackects = 1;

        while (++position < text.Length && brackects > 0)
        {
          if (text[position] == '(')
          {
            ++brackects;
          }
          else if (text[position] == ')')
          {
            --brackects;
          }

          if (brackects > 0)
          {
            builder.Append(text[position]);
          }
        }

        token = builder.ToString();
        builder.Clear();

        yield return token;
      }
    }
  }
}
