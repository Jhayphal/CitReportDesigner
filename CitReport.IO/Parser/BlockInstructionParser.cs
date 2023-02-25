namespace CitReport.IO.Parser;

/// <summary>
/// For instruction started with '{'
/// </summary>
public abstract class BlockInstructionParser : IInstructionParser
{
  protected readonly InstructionTokenizer Tokenizer = new();

  protected abstract CodeContext ActualContext { get; }

  protected abstract IEnumerable<string> SupportedInstructions { get; }

  protected static bool IsInstructionSupported(string current, IEnumerable<string> instructions)
  {
    int position = 1;

    while (position < current.Length && char.IsWhiteSpace(current[position]))
    {
      ++position;
    }

    foreach (var supportedInstruction in instructions)
    {
      if (current.IndexOf(supportedInstruction, StringComparison.OrdinalIgnoreCase) == position)
      {
        return true;
      }
    }

    return false;
  }

  protected static bool IsInstructionSupported(string current, string instruction)
  {
    int position = 1;

    while (position < current.Length && char.IsWhiteSpace(current[position]))
    {
      ++position;
    }

    if (current.IndexOf(instruction) == position)
    {
      return true;
    }

    return false;
  }

  public bool CanParse(string current, CodeContext context)
    => context == ActualContext
      && current.Length > 1
      && current[0] == '{'
      && IsInstructionSupported(current, SupportedInstructions);

  public abstract void Parse(ParserContext context, string current);
}
