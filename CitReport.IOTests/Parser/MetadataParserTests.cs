using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests
{
  [TestClass]
  public class MetadataParserTests
  {
    private readonly Dictionary<string, string> validCases = new()
    {
      { "{/* test */}", " test " },
      { "{/*PAGESIZE 210x290*/}\r\n", "PAGESIZE 210x290" },
      { "{/*BlockHeaderColor:=227,197,186*/}", "BlockHeaderColor:=227,197,186" },
      { "{/*MARGT 0 ", "MARGT 0 " }
    };

    private readonly List<string> wrongCases = new()
    {
      "{/TS..",
      "/BLK RH",
      "",
      null,
      "some text",
      "/",
      " "
    };

    [TestMethod]
    public void CanParseTests()
    {
      var parser = new MetadataParser();

      foreach (var testCase in validCases)
      {
        Assert.IsTrue(parser.CanParse(testCase.Key, CodeContext.CodeBehind));
      }

      foreach (var testCase in validCases)
      {
        Assert.IsTrue(parser.CanParse(testCase.Key, CodeContext.Block));
      }

      foreach (var testCase in validCases)
      {
        Assert.IsFalse(parser.CanParse(testCase.Key, CodeContext.ReportDefinition));
      }

      foreach (var testCase in wrongCases)
      {
        Assert.IsFalse(parser.CanParse(testCase, CodeContext.CodeBehind));
      }

      foreach (var testCase in wrongCases)
      {
        Assert.IsFalse(parser.CanParse(testCase, CodeContext.Block));
      }
    }

    [TestMethod]
    public void ParseTests()
    {
      var errorProvider = new TestErrorProvider();
      
      var context = new ParserContext(errorProvider);
      context.SetContext(CodeContext.CodeBehind);
      
      var parser = new MetadataParser();
      foreach (var testCase in validCases)
      {
        parser.Parse(context, testCase.Key);
        var actual = context.Report.Metadata.Last();
        var expected = new Metadata { Value = testCase.Value };
        
        Assert.AreEqual(expected, actual);
      }
    }
  }
}