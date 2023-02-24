using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests;

[TestClass()]
public class DoParserTests : InstructionParserTestsBase<DoParser>
{
  protected override List<string> ValidCases { get; } = new()
  {
    "/DO r_u[5]:=rdic(\"C_VAL\",PRED->KODS,,\"{||KODV}\") //·¦¯ ª¨væêv",
    "/do  text "
  };

  protected override List<string> WrongCases { get; } = new()
  {
    " /DO r_u[2]:=q_u[3]\r\n",
    "/ DO r_u[2]:=q_u[3]\r\n",
    "/BLK PH BlkH(0)\r\n",
    "/ FIELD"
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
  public void Parse_Expression()
  {
    var expression = "r_u[5]:=rdic(\"C_VAL\",PRED->KODS,,\"{||KODV}\") //·¦¯ ª¨væêv\r\n";
    var testCase = $"/Do  {expression}";

    ErrorProvider.Errors.Clear();
    Parser.Parse(Context, testCase);

    var actual = Context.Report.Definition.DoActions.LastOrDefault();
    var expected = new Expression { Value = expression };

    Assert.AreEqual(expected, actual);
    Assert.AreEqual(0, ErrorProvider.Errors.Count);
  }
}