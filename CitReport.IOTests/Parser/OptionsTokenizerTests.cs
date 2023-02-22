using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests
{
  [TestClass()]
  public class OptionsTokenizerTests
  {
    [TestMethod()]
    public void GetTokens_SingleOption_WithoutParameters()
    {
      var tokenizer = new OptionsTokenizer("Details()");
      var actual = tokenizer.GetTokens().ToArray();
      var expected = new string[] { "Details", string.Empty };

      actual.AreEquals(expected);
    }

    [TestMethod()]
    public void GetTokens_SingleOption_WithoutParameters_WithSpaces()
    {
      var tokenizer = new OptionsTokenizer("   Details()   ");
      var actual = tokenizer.GetTokens().ToArray();
      var expected = new string[] { "Details", string.Empty };

      actual.AreEquals(expected);
    }

    [TestMethod()]
    public void GetTokens_SingleOption_SpaceInParameters()
    {
      var tokenizer = new OptionsTokenizer("Details( )");
      var actual = tokenizer.GetTokens().ToArray();
      var expected = new string[] { "Details", " " };

      actual.AreEquals(expected);
    }

    [TestMethod()]
    public void GetTokens_SingleOption_SingleParameter()
    {
      var tokenizer = new OptionsTokenizer("BlkH(100)");
      var actual = tokenizer.GetTokens().ToArray();
      var expected = new string[] { "BlkH", "100" };

      actual.AreEquals(expected);
    }

    [TestMethod()]
    public void GetTokens_ManyOptions_WithoutParameters()
    {
      var tokenizer = new OptionsTokenizer("Cond() Totals() Details()");
      var actual = tokenizer.GetTokens().ToArray();
      var expected = new string[] { "Cond", string.Empty, "Totals", string.Empty, "Details", string.Empty };

      actual.AreEquals(expected);
    }

    [TestMethod()]
    public void GetTokens_ManyOptions_WithParameters()
    {
      var tokenizer = new OptionsTokenizer("Cond(100 > 1) BlkH(30)");
      var actual = tokenizer.GetTokens().ToArray();
      var expected = new string[] { "Cond", "100 > 1", "BlkH", "30" };

      actual.AreEquals(expected);
    }

    [TestMethod()]
    public void GetTokens_SingleOption_WithExpressionParameter()
    {
      var tokenizer = new OptionsTokenizer("BREAK(IIF(x > 1, 2, 4))");
      var actual = tokenizer.GetTokens().ToArray();
      var expected = new string[] { "BREAK", "IIF(x > 1, 2, 4)" };

      actual.AreEquals(expected);
    }
  }
}