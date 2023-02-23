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

    private readonly TestErrorProvider errorProvider = new();
    private readonly ReportDefinitionParser parser = new();
    private readonly ParserContext context;

    public ReportDefinitionParserTests()
    {
      context = new ParserContext(errorProvider);
    }

    [TestMethod()]
    public void CanParseTests()
    {
      foreach (var testCase in validCases)
      {
        Assert.IsTrue(parser.CanParse(testCase, CodeContext.CodeBehind));
      }

      foreach (var testCase in validCases)
      {
        Assert.IsFalse(parser.CanParse(testCase, CodeContext.ReportDefinition));
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
      var repCode = "Rep";
      var testCase = $"/REPORT {repCode}";

      errorProvider.Errors.Clear();
      parser.Parse(context, testCase);
      
      Assert.AreEqual(CodeContext.ReportDefinition, context.Context);

      var actual = context.Report.Definition;
      var expected = new ReportBlock { Code = repCode };

      Assert.AreEqual(expected, actual);
      Assert.AreEqual(0, errorProvider.Errors.Count);
    }

    [TestMethod()]
    public void Parse_WithOptions()
    {
      var repCode = "ZP10SFRD";
      var testCase = $"/REPORT {repCode} Ctl(Str(CEX, 4) + Str(UCH, 4)) Skipper(SkipRecord())";
      
      errorProvider.Errors.Clear();
      parser.Parse(context, testCase);

      Assert.AreEqual(CodeContext.ReportDefinition, context.Context);

      var actual = context.Report.Definition;
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