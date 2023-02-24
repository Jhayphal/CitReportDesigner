using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests;

[TestClass()]
public class AfterEndParserTests : InstructionParserTestsBase<AfterEndParser>
{
  protected override List<string> ValidCases { get; } = new()
  {
    "/AFTER END r_u := Azer(12)",
    "/after end r_u := Azer(12)",
    "/After End  r_u := Azer(12) "
  };

  protected override List<string> WrongCases { get; } = new()
  {
    "/REPORT repCode",
    "/BLK repCode",
    "{/*repCode*/}",
    "/AFTER START r_u := Azer(12)",
    " /AFTER END r_u := Azer(12)",
    "/AFTER  END r_u := Azer(12) "
  };

  [TestMethod()]
  public void CanParseTest()
  {
    CanParse(ValidCases, CodeContext.ReportDefinition, expected: true);
    CanParse(ValidCases, CodeContext.CodeBehind, expected: false);
    CanParse(ValidCases, CodeContext.Block, expected: false);
    CanParse(WrongCases, CodeContext.ReportDefinition, expected: false);
  }

  [TestMethod()]
  public void ParseTest()
  {
    var expression = "CloseReport( )  ";
    var testCase = $"/AFTER END  {expression}";

    ErrorProvider.Errors.Clear();
    Parser.Parse(Context, testCase);

    var actual = Context.Report.Definition.AfterEndActions.LastOrDefault();
    var expected = new Expression { Value = expression };

    Assert.AreEqual(expected, actual);
  }
}