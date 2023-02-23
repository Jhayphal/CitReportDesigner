using System.Text;

namespace CitReport.IO.Parser;

public class InstructionTokenizer
{
  private readonly HashSet<char> breakers;

  public InstructionTokenizer()
  {
    breakers = new() { '{', ',', ':', '}' };
  }

  public InstructionTokenizer(IEnumerable<char> splitBy)
  {
    breakers = new(splitBy);
  }

  public IEnumerable<string> GetTokens(string current)
  {
    if (current == null)
    {
      yield break;
    }

    var builder = new StringBuilder();
    var position = 0;
    var definitionEnded = false;

    do
    {
      while (position < current.Length && (definitionEnded || !breakers.Contains(current[position])))
      {
        builder.Append(current[position++]);
      }

      if (builder.Length > 0)
      {
        var result = builder.ToString();
        builder.Clear();
        yield return result;
      }

      if (position < current.Length)
      {
        var breaker = current[position++];
        if (!definitionEnded && breaker == '}')
        {
          definitionEnded = true;
        }
        yield return breaker.ToString();
      }
    }
    while (position < current.Length);
  }
}
