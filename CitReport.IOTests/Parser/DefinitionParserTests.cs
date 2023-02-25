using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests;

[TestClass()]
public class DefinitionParserTests : InstructionParserTestsBase<DefinitionParser>
{
  protected override List<string> ValidCases { get; } = new()
  {
    "/REPORT repCode",
    "/REPORT repCode Ctl(Str(CEX, 4) + Str(UCH, 4)) Skipper(SkipRecord())",
    "/REPORT   A2   BREAK(CEX, UCH)"
  };

  protected override List<string> WrongCases { get; } = new()
  {
    " /REPORT repCode",
    "/BLK repCode",
    "{/*repCode*/}"
  };

  [TestMethod()]
  public void CanParseTests()
  {
    CanParse(ValidCases, CodeContext.CodeBehind, expected: true);
    CanParse(ValidCases, CodeContext.ReportDefinition, expected: false);
    CanParse(ValidCases, CodeContext.Block, expected: false);
    CanParse(WrongCases, CodeContext.ReportDefinition, expected: false);
  }

  [TestMethod()]
  public void Parse_WithoutOptions()
  {
    var repCode = "Rep";
    var testCase = $"/REPORT {repCode}";

    ErrorProvider.Errors.Clear();
    Parser.Parse(Context, testCase);

    var actual = Context.Report.Definition;
    var expected = new ReportBlock { Code = repCode };
    
    Assert.AreEqual(expected, actual);
    Assert.AreEqual(CodeContext.ReportDefinition, Context.CodeContext);
    Assert.AreEqual(0, ErrorProvider.Errors.Count);
  }

  [TestMethod()]
  public void Parse_WithOptions()
  {
    var repCode = "ZP10SFRD";
    var testCase = $"/REPORT {repCode} Ctl(Str(CEX, 4) + Str(UCH, 4)) Skipper(SkipRecord())";
    
    ErrorProvider.Errors.Clear();
    Parser.Parse(Context, testCase);

    var actual = Context.Report.Definition;
    var expected = new ReportBlock { Code = repCode };
    expected.Options.AddRange(new Option[]
    {
      new CtlOption { Name = "Ctl", Value = "Str(CEX, 4) + Str(UCH, 4)" },
      new SkipperOption { Name = "Skipper", Value = "SkipRecord()" }
    });

    Assert.AreEqual(expected, actual);
    Assert.AreEqual(CodeContext.ReportDefinition, Context.CodeContext);
    Assert.AreEqual(0, ErrorProvider.Errors.Count);
  }
}