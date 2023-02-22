using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CitReport.IO.Parser.Tests
{
  [TestClass()]
  public class BlockInstructionTokenizerTests
  {
    [TestMethod()]
    public void GetTokens_Null()
    {
      var tokenizer = new BlockInstructionTokenizer();
      var actual = tokenizer.GetTokens(null).ToArray();
      var expected = Array.Empty<string>();

      expected.AreEquals(actual);
    }

    [TestMethod()]
    public void GetTokens_Empty()
    {
      var tokenizer = new BlockInstructionTokenizer();
      var actual = tokenizer.GetTokens(string.Empty).ToArray();
      var expected = Array.Empty<string>();

      expected.AreEquals(actual);
    }

    [TestMethod()]
    public void GetTokens_Whitespaces()
    {
      var tokenizer = new BlockInstructionTokenizer();
      var actual = tokenizer.GetTokens("    ").ToArray();
      var expected = new string[] { "    " };

      expected.AreEquals(actual);
    }

    [TestMethod()]
    public void GetTokens_Ct()
    {
      var tokenizer = new BlockInstructionTokenizer();
      var actual = tokenizer.GetTokens($"{{{Instructions.Ct}, 0, 54, 255}}").ToArray();
      var expected = new string[] { "{", Instructions.Ct, ",", " 0", ",", " 54", ",", " 255", "}" };

      expected.AreEquals(actual);
    }

    [TestMethod()]
    public void GetTokens_Ts()
    {
      var tokenizer = new BlockInstructionTokenizer();
      var actual = tokenizer.GetTokens($"{{{Instructions.Ts}, B1:B1, , FONT1}}SOM ___").ToArray();
      var expected = new string[] { "{", Instructions.Ts, ",", " B1", ":", "B1", ",", " ", ",", " FONT1", "}", "SOM ___" };

      expected.AreEquals(actual);
    }
  }
}