using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests;

[TestClass]
public class MetadataParserTests : InstructionParserTestsBase<MetadataParser>
{
  private readonly Dictionary<string, string> validCases = new()
  {
    { "{/* test */}", " test " },
    { "{/*PAGESIZE 210x290*/}\r\n", "PAGESIZE 210x290" },
    { "{/*BlockHeaderColor:=227,197,186*/}", "BlockHeaderColor:=227,197,186" },
    { "{/*MARGT 0 ", "MARGT 0 " }
  };

  protected override List<string> ValidCases => validCases.Keys.ToList();

  protected override List<string> WrongCases { get; } = new()
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
    CanParse(ValidCases, CodeContext.CodeBehind, expected: true);
    CanParse(ValidCases, CodeContext.Block, expected: true);
    CanParse(ValidCases, CodeContext.ReportDefinition, expected: false);
    CanParse(WrongCases, CodeContext.CodeBehind, expected: false);
    CanParse(WrongCases, CodeContext.Block, expected: false);
  }

  [TestMethod]
  public void ParseTests()
  {
    Context.SetContext(CodeContext.CodeBehind);
    ErrorProvider.Errors.Clear();

    foreach (var testCase in validCases)
    {
      Parser.Parse(Context, testCase.Key);
      var actual = Context.Report.Metadata.Last();
      var expected = new Metadata { Value = testCase.Value };
      
      Assert.AreEqual(expected, actual);
      Assert.AreEqual(0, ErrorProvider.Errors.Count);
    }
  }
}