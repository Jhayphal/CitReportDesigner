using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests
{
  [TestClass()]
  public class AfterStartParserTests
  {
    private readonly List<string> validCases = new()
    {
      "/AFTER START r_u := Azer(12)",
      "/after start r_u := Azer(12)",
      "/After Start  r_u := Azer(12) "
    };

    private readonly List<string> wrongCases = new()
    {
      "/REPORT repCode",
      "/BLK repCode",
      "{/*repCode*/}",
      " /AFTER START r_u := Azer(12)"
    };

    private readonly TestErrorProvider errorProvider = new();
    private readonly AfterStartParser parser = new();
    private readonly ParserContext context;

    public AfterStartParserTests()
    {
      context = new ParserContext(errorProvider);
    }

    [TestMethod()]
    public void CanParseTest()
    {
      foreach (var testCase in validCases)
      {
        Assert.IsTrue(parser.CanParse(testCase, CodeContext.ReportDefinition));
      }

      foreach (var testCase in validCases)
      {
        Assert.IsFalse(parser.CanParse(testCase, CodeContext.CodeBehind));
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
    public void Parse_Expression()
    {
      var expression = "R_U[1] := Substr(_NTrim(PST->YEAR), 3, 2) + StrZ(PST->MON, 2) + _NTrim(PST->PAKET)   ";
      var testCase = $"/AFTER START   {expression}";

      errorProvider.Errors.Clear();
      parser.Parse(context, testCase);

      var actual = context.Report.Definition.AfterStartActions.LastOrDefault();
      var expected = new Expression { Value = expression };
      
      Assert.AreEqual(expected, actual);
    }
  }
}