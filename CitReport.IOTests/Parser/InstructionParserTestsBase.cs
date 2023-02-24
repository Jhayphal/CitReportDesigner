using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests;

public abstract class InstructionParserTestsBase<TParser>
  where TParser : IInstructionParser, new()
{
  protected abstract List<string> ValidCases { get; }

  protected abstract List<string> WrongCases { get; }

  protected readonly TestErrorProvider ErrorProvider = new();
  protected readonly TParser Parser = new();
  protected readonly ParserContext Context;

  protected InstructionParserTestsBase()
  {
    Context = new ParserContext(ErrorProvider);
  }

  protected void CanParse(IEnumerable<string> cases, CodeContext context, bool expected)
  {
    Action<bool> assert = expected
      ? Assert.IsTrue
      : Assert.IsFalse;

    foreach (var testCase in cases)
    {
      assert(Parser.CanParse(testCase, context));
    }
  }
}
