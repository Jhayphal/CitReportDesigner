using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests
{
  [TestClass()]
  public class OptionsParserTests
  {
    [TestMethod()]
    public void Parse_ManyOptions_WithParams()
    {
      var parser = new OptionsParser();
      var actual = parser.Parse("BREAK(TEXT, CODE) Ctl(PAGE_N) Skipper(SkipRecord())", new DebugErrorProvider()).ToArray();
      var expected = new Option[]
      {
        new BreakOption { Name = "BREAK", Value = "TEXT, CODE" },
        new CtlOption { Value = "PAGE_N" },
        new SkipperOption { Value = "SkipRecord()" }
      };

      expected.AreEquals(actual);
    }
  }
}