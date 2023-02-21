namespace CitReport.Services.Parser
{
  internal interface IInstructionParser
  {
    bool CanParse(string current, CodeContext context);

    void Parse(ParserContext context, string current);
  }
}
