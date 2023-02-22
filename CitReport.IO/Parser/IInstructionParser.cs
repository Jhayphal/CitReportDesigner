namespace CitReport.IO.Parser;

public interface IInstructionParser
{
  bool CanParse(string current, CodeContext context);

  void Parse(ParserContext context, string current);
}
