﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests;

[TestClass()]
public class AfterStartParserTests : InstructionParserTestsBase<AfterStartParser>
{
  protected override List<string> ValidCases { get; } = new()
  {
    "/AFTER START r_u := Azer(12)",
    "/after start r_u := Azer(12)",
    "/After Start  r_u := Azer(12) "
  };

  protected override List<string> WrongCases { get; } = new()
  {
    "/REPORT repCode",
    "/BLK repCode",
    "{/*repCode*/}",
    " /AFTER START r_u := Azer(12)",
    "/AFTER  START r_u := Azer(12)",
    "/AFTER END r_u := Azer(12)",
    "/ 'text'"
  };

  [TestMethod()]
  public void CanParseTests()
  {
    CanParse(ValidCases, CodeContext.ReportDefinition, expected: true);
    CanParse(ValidCases, CodeContext.CodeBehind, expected: false);
    CanParse(ValidCases, CodeContext.Block, expected: false);
    CanParse(WrongCases, CodeContext.ReportDefinition, expected: false);
  }

  [TestMethod()]
  public void Parse_Expression()
  {
    var expression = "R_U[1] := Substr(_NTrim(PST->YEAR), 3, 2) + StrZ(PST->MON, 2) + _NTrim(PST->PAKET)   ";
    var testCase = $"/AFTER START   {expression}";

    ErrorProvider.Errors.Clear();
    Parser.Parse(Context, testCase);

    var actual = Context.Report.Definition.AfterStartActions.LastOrDefault();
    var expected = new Expression { Value = expression };

    Assert.AreEqual(expected, actual);
    Assert.AreEqual(0, ErrorProvider.Errors.Count);
  }
}