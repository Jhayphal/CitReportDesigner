using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests
{
  [TestClass()]
  public class ReportDefinitionParserTests
  {
    private readonly List<string> validCases = new()
    {
      "/REPORT repCode",
      "/REPORT repCode Ctl(Str(CEX, 4) + Str(UCH, 4)) Skipper(SkipRecord())",
      "/REPORT   A2   BREAK(CEX, UCH)"
    };

    private readonly List<string> wrongCases = new()
    {
      " /REPORT repCode",
      "/BLK repCode",
      "{/*repCode*/}"
    };

    [TestMethod()]
    public void CanParseTests()
    {
      var parser = new ReportDefinitionParser();
      foreach (var testCase in validCases)
      {
        Assert.IsTrue(parser.CanParse(testCase, CodeContext.ReportDefinition));
      }

      foreach (var testCase in validCases)
      {
        Assert.IsTrue(parser.CanParse(testCase, CodeContext.CodeBehind));
      }

      foreach (var testCase in validCases)
      {
        Assert.IsFalse(parser.CanParse(testCase, CodeContext.Block));
      }

      foreach (var testCase in wrongCases)
      {
        Assert.IsFalse(parser.CanParse(testCase, CodeContext.ReportDefinition));
      }
    }

    [TestMethod()]
    public void Parse_WithoutOptions()
    {
      var errorProvider = new TestErrorProvider();
      var context = new ParserContext(errorProvider);

      var repCode = "Rep";
      var testCase = $"/REPORT {repCode}";
      var parser = new ReportDefinitionParser();
      parser.Parse(context, testCase);
      
      var actual = context.Report.ReportDefinition;
      var expected = new ReportBlock { Code = repCode };

      Assert.AreEqual(expected, actual);
      Assert.AreEqual(0, errorProvider.Errors.Count);
    }

    [TestMethod()]
    public void Parse_WithOptions()
    {
      var errorProvider = new TestErrorProvider();
      var context = new ParserContext(errorProvider);

      var repCode = "ZP10SFRD";
      var testCase = $"/REPORT {repCode} Ctl(Str(CEX, 4) + Str(UCH, 4)) Skipper(SkipRecord())";
      var parser = new ReportDefinitionParser();
      parser.Parse(context, testCase);

      var actual = context.Report.ReportDefinition;
      var expected = new ReportBlock { Code = repCode };
      expected.Options.AddRange(new Option[]
      {
        new CtlOption { Name = "Ctl", Value = "Str(CEX, 4) + Str(UCH, 4)" },
        new SkipperOption { Name = "Skipper", Value = "SkipRecord()" }
      });

      Assert.AreEqual(expected, actual);
      Assert.AreEqual(0, errorProvider.Errors.Count);
    }
  }
}