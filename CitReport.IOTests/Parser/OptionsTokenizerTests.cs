using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests;

[TestClass()]
public class OptionsTokenizerTests
{
  private readonly OptionsTokenizer tokenizer = new();

  [TestMethod()]
  public void GetTokens_Null()
  {
    var actual = tokenizer.GetTokens(null).ToArray();
    var expected = Array.Empty<string>();

    expected.AreEquals(actual);
  }


  [TestMethod()]
  public void GetTokens_Empty()
  {
    var actual = tokenizer.GetTokens(string.Empty).ToArray();
    var expected = Array.Empty<string>();

    expected.AreEquals(actual);
  }

  [TestMethod()]
  public void GetTokens_Whitespaces()
  {
    var actual = tokenizer.GetTokens("    ").ToArray();
    var expected = Array.Empty<string>();

    expected.AreEquals(actual);
  }

  [TestMethod()]
  public void GetTokens_SingleOption_WithoutParameters()
  {
    var actual = tokenizer.GetTokens("Details()").ToArray();
    var expected = new string[] { "Details", string.Empty };

    expected.AreEquals(actual);
  }

  [TestMethod()]
  public void GetTokens_SingleOption_WithoutParameters_WithSpaces()
  {
    var actual = tokenizer.GetTokens("   Details()   ").ToArray();
    var expected = new string[] { "Details", string.Empty };

    expected.AreEquals(actual);
  }

  [TestMethod()]
  public void GetTokens_SingleOption_SpaceInParameters()
  {
    var actual = tokenizer.GetTokens("Details( )").ToArray();
    var expected = new string[] { "Details", " " };

    expected.AreEquals(actual);
  }

  [TestMethod()]
  public void GetTokens_SingleOption_SingleParameter()
  {
    var actual = tokenizer.GetTokens("BlkH(100)").ToArray();
    var expected = new string[] { "BlkH", "100" };

    expected.AreEquals(actual);
  }

  [TestMethod()]
  public void GetTokens_ManyOptions_WithoutParameters()
  {
    var actual = tokenizer.GetTokens("Cond() Totals() Details()").ToArray();
    var expected = new string[] { "Cond", string.Empty, "Totals", string.Empty, "Details", string.Empty };

    expected.AreEquals(actual);
  }

  [TestMethod()]
  public void GetTokens_ManyOptions_WithParameters()
  {
    var actual = tokenizer.GetTokens("Cond(100 > 1) BlkH(30)").ToArray();
    var expected = new string[] { "Cond", "100 > 1", "BlkH", "30" };

    expected.AreEquals(actual);
  }

  [TestMethod()]
  public void GetTokens_SingleOption_WithExpressionParameter()
  {
    var actual = tokenizer.GetTokens("BREAK(IIF(x > 1, 2, 4))").ToArray();
    var expected = new string[] { "BREAK", "IIF(x > 1, 2, 4)" };

    expected.AreEquals(actual);
  }

  [TestMethod()]
  public void GetTokens_SingleOption_WithUnfinishedExpressionParameter()
  {
    var actual = tokenizer.GetTokens("BREAK(IIF(x > 1, 2").ToArray();
    var expected = new string[] { "BREAK", "IIF(x > 1, 2" };

    expected.AreEquals(actual);
  }

  [TestMethod()]
  public void GetTokens_UnknownOption_WithExpressionParameter()
  {
    var actual = tokenizer.GetTokens("Unknown(x > 1)").ToArray();
    var expected = new string[] { "Unknown", "x > 1" };

    expected.AreEquals(actual);
  }

  [TestMethod()]
  public void GetTokens_UnknownOption_WithoutParameters()
  {
    var actual = tokenizer.GetTokens("Unknown()").ToArray();
    var expected = new string[] { "Unknown", string.Empty };

    expected.AreEquals(actual);
  }

  [TestMethod()]
  public void GetTokens_EmptyBrackets()
  {
    var actual = tokenizer.GetTokens("()").ToArray();
    var expected = new string[] { string.Empty, string.Empty };

    expected.AreEquals(actual);
  }
}