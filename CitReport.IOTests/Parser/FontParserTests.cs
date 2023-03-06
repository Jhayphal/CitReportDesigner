using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests;

[TestClass()]
public class FontParserTests : InstructionParserTestsBase<FontParser>
{
  protected override List<string> ValidCases { get; } = new()
  {
    "{/FL, FONT1, Arial, 10, B}",
    "{/FL , FONT1, Arial, 10}",
    "{/fl, FONT1, Arial, 10}"
  };

  protected override List<string> WrongCases { get; } = new()
  {
    " {/FL, FONT1, Arial, 10}",
    "/FL , FONT1, Arial, 10}",
    "(/FL , FONT1, Arial, 10}",
    "{/F"
  };

  [TestMethod()]
  public void CanParseTests()
  {
    CanParse(ValidCases, CodeContext.Block, expected: true);
    CanParse(ValidCases, CodeContext.ReportDefinition, expected: false);
    CanParse(ValidCases, CodeContext.CodeBehind, expected: false);
    CanParse(WrongCases, CodeContext.Block, expected: false);
  }

  [TestMethod()]
  public void Parse_Expression()
  {
    var body = ", FONT1, Arial, 10, B}";
    var testCase = $"{{/FL {body}";

    var testBlock = new BodyBlock(Context.Report);
    PrepareContext(testBlock);
    Parser.Parse(Context, testCase);

    var actual = testBlock.Fonts.LastOrDefault();
    var expected = new KeyValuePair<string, FontInfo>(
      "FONT1", 
      new FontInfo
      {
        Family = "Arial",
        Size = 10f,
        Style = FontStyle.Bold
      });

    Assert.AreEqual(expected, actual);
    Assert.AreEqual(0, ErrorProvider.Errors.Count);
  }

  private void PrepareContext(BodyBlock testBlock)
  {
    ErrorProvider.Errors.Clear();
    Context.SetContext(CodeContext.Block);
    Context.Report.Blocks.Add(testBlock);
    Context.SetBlockAsCurrent(testBlock);
  }
}