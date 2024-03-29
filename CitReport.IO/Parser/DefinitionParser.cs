﻿namespace CitReport.IO.Parser;

public class DefinitionParser : IInstructionParser
{
  private readonly InstructionTokenizer tokenizer = new(new char[] { ' ' });
  private readonly OptionsParser optionsParser = new();

  public bool CanParse(string current, CodeContext context)
    => context == CodeContext.CodeBehind
      && current.StartsWith(Instructions.Report, StringComparison.OrdinalIgnoreCase);

  public void Parse(ParserContext context, string current)
  {
    var code = tokenizer.GetTokens(current)
      .Skip(1)
      .Where(t => !string.IsNullOrWhiteSpace(t))
      .FirstOrDefault();

    if (code is null)
    {
      context.ErrorProvider.UnfinishedInstruction(current, context.CurrentLine);
      return;
    }

    context.Report.Definition.Code = code;
    context.SetContext(CodeContext.ReportDefinition);

    var indexOfCode = current.IndexOf(code);
    var startOptionsIndex = indexOfCode + code.Length;

    if (startOptionsIndex < current.Length)
    {
      var options = optionsParser.Parse(context, current[startOptionsIndex..]);
      context.Report.Definition.Options.AddRange(options);
    }
  }
}
