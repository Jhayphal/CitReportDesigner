using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests
{
  [TestClass()]
  public class OptionsParserTests
  {
    [TestMethod()]
    public void Parse_Null()
    {
      var parser = new OptionsParser();
      var actual = parser.Parse(null, new DebugErrorProvider()).ToArray();
      var expected = Array.Empty<Option>();

      expected.AreEquals(actual);
    }

    [TestMethod()]
    public void Parse_Empty()
    {
      var parser = new OptionsParser();
      var actual = parser.Parse(string.Empty, new DebugErrorProvider()).ToArray();
      var expected = Array.Empty<Option>();

      expected.AreEquals(actual);
    }

    [TestMethod()]
    public void Parse_Whitespaces()
    {
      var parser = new OptionsParser();
      var actual = parser.Parse("    ", new DebugErrorProvider()).ToArray();
      var expected = Array.Empty<Option>();

      expected.AreEquals(actual);
    }

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

    [TestMethod()]
    public void Parse_UnknownOption_WithParameter()
    {
      var errorProvider = new TestErrorProvider();
      var parser = new OptionsParser();
      var actual = parser.Parse("Unknown( TEXT )", errorProvider).ToArray();
      var expected = new Option[]
      {
        new Option { Name = "Unknown", Value = " TEXT " }
      };

      expected.AreEquals(actual);

      Assert.AreEqual(1, errorProvider.Errors.Count);
    }
  }
}