using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests;

[TestClass()]
public class BodyBlockParserTests : InstructionParserTestsBase<BodyBlockParser>
{
  protected override List<string> ValidCases { get; } = new()
  {
    "/BLK PH BlkH(0)",
    "/blk PH"
  };

  protected override List<string> WrongCases { get; } = new()
  {
    "/ BLK PH",
    " /BLK PH",
    "/BLKPH"
  };

  [TestMethod()]
  public void CanParseTests()
  {
    CanParse(ValidCases, CodeContext.ReportDefinition, expected: true);
    CanParse(ValidCases, CodeContext.Block, expected: true);
    CanParse(ValidCases, CodeContext.CodeBehind, expected: false);
    CanParse(WrongCases, CodeContext.ReportDefinition, expected: false);
    CanParse(WrongCases, CodeContext.Block, expected: false);
  }

  [TestMethod()]
  public void Parse_WithOption()
  {
    var code = "PH";
    var optionName = "BlkH";
    var optionValue = "15";
    var expression = $"{code} {optionName}({optionValue})";
    var testCase = $"/BLK  {expression}";

    ErrorProvider.Errors.Clear();
    Context.SetContext(CodeContext.ReportDefinition);
    Parser.Parse(Context, testCase);

    var actual = Context.Report.Blocks.LastOrDefault();
    var expected = new BodyBlock(Context.Report, id: actual.Id) { Code = code };
    expected.Options.Add(new BlkHOption { Name = optionName, Value = optionValue });

    Assert.AreEqual(expected, actual);
    Assert.AreEqual(CodeContext.Block, Context.CodeContext);
    Assert.IsTrue(ReferenceEquals(actual, Context.CurrentBlock));
    Assert.AreEqual(0, ErrorProvider.Errors.Count);
  }
}