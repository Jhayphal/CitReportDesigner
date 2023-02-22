namespace CitReport.Services.Parser
{
  internal class FieldParser : IInstructionParser
  {
    public bool CanParse(string current, CodeContext context) => context == CodeContext.Block
      && current.Length > 0
      && current[0] == '/'
      && (current.Length < 2 || current[1] != '/');

    public void Parse(ParserContext context, string current)
    {
      var valueStorage = context.Fields.Dequeue();
      valueStorage.Target.SetValue(valueStorage.Language, current[1..].TrimStart());
    }
  }
}
