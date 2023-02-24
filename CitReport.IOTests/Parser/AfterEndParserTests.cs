using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests;

[TestClass()]
public class AfterEndParserTests
{
  private readonly List<string> validCases = new()
  {
    "/AFTER END r_u := Azer(12)",
    "/after end r_u := Azer(12)",
    "/After End  r_u := Azer(12) "
  };

  private readonly List<string> wrongCases = new()
  {
    "/REPORT repCode",
    "/BLK repCode",
    "{/*repCode*/}",
    "/AFTER START r_u := Azer(12)",
    " /AFTER END r_u := Azer(12)",
    "/AFTER  END r_u := Azer(12) "
  };

  private readonly TestErrorProvider errorProvider = new();
  private readonly AfterEndParser parser = new();
  private readonly ParserContext context;

  public AfterEndParserTests()
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
  public void ParseTest()
  {
    var expression = "CloseReport( )  ";
    var testCase = $"/AFTER END  {expression}";

    errorProvider.Errors.Clear();
    parser.Parse(context, testCase);

    var actual = context.Report.Definition.AfterEndActions.LastOrDefault();
    var expected = new Expression { Value = expression };

    Assert.AreEqual(expected, actual);
  }
}