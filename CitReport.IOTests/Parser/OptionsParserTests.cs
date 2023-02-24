using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests;

[TestClass()]
public class OptionsParserTests
{
  private readonly OptionsParser parser = new();
  private readonly TestErrorProvider errorProvider = new();
  private readonly ParserContext context;

  public OptionsParserTests()
  {
    context = new ParserContext(errorProvider);
  }

  [TestMethod()]
  public void Parse_Null()
  {
    errorProvider.Errors.Clear();

    var actual = parser.Parse(context, null).ToArray();
    var expected = Array.Empty<Option>();

    expected.AreEquals(actual);
    Assert.AreEqual(0, errorProvider.Errors.Count);
  }

  [TestMethod()]
  public void Parse_Empty()
  {
    errorProvider.Errors.Clear();

    var actual = parser.Parse(context, string.Empty).ToArray();
    var expected = Array.Empty<Option>();

    expected.AreEquals(actual);
    Assert.AreEqual(0, errorProvider.Errors.Count);
  }

  [TestMethod()]
  public void Parse_Whitespaces()
  {
    errorProvider.Errors.Clear();

    var actual = parser.Parse(context, "    ").ToArray();
    var expected = Array.Empty<Option>();

    expected.AreEquals(actual);
    Assert.AreEqual(0, errorProvider.Errors.Count);
  }

  [TestMethod()]
  public void Parse_ManyOptions_WithParams()
  {
    errorProvider.Errors.Clear();

    var actual = parser.Parse(context, "BREAK(TEXT, CODE) Ctl(PAGE_N) Skipper(SkipRecord())").ToArray();
    var expected = new Option[]
    {
      new BreakOption { Name = "BREAK", Value = "TEXT, CODE" },
      new CtlOption { Value = "PAGE_N" },
      new SkipperOption { Value = "SkipRecord()" }
    };

    expected.AreEquals(actual);
    Assert.AreEqual(0, errorProvider.Errors.Count);
  }

  [TestMethod()]
  public void Parse_UnknownOption_WithParameter()
  {
    errorProvider.Errors.Clear();

    var actual = parser.Parse(context, "Unknown( TEXT )").ToArray();
    var expected = new Option[]
    {
      new Option { Name = "Unknown", Value = " TEXT " }
    };

    expected.AreEquals(actual);
    Assert.AreEqual(1, errorProvider.Errors.Count);
  }
}