namespace CitReport.IO.Parser;

internal static class ParserErrorExtensions
{
  public static void SomeFieldsHasNotValues(this IErrorProvider errorProvider, int fieldsCount, int blockNumber)
    => errorProvider.AddError($"{fieldsCount} fields has not values in block {blockNumber}.");

  public static void UnfinishedInstruction(this IErrorProvider errorProvider, string instruction, int line)
    => errorProvider.AddError($"Unfinished instruction: '{instruction}' at line {line}.");

  public static void OptionHasNotValue(this IErrorProvider errorProvider, string optionName, int line)
    => errorProvider.AddError($"Option '{optionName}' has not value at line {line}.");

  public static void UnsupportedOption(this IErrorProvider errorProvider, string optionName, int line)
    => errorProvider.AddError($"Unsupported option '{optionName}' at line {line}.");

  public static void WrongType(this IErrorProvider errorProvider, Type optionType, int line)
    => errorProvider.AddError($"Type '{optionType.FullName}' does not inherit Option type at line {line}.");

  public static void UnexpectedTableInstructionLocation(this IErrorProvider errorProvider, string instruction, int line)
    => errorProvider.AddError($"Unexpected table instruction '{instruction}' outside table definition at line {line}.");

  public static void WrongInstructionBody(this IErrorProvider errorProvider, string instruction, int line)
    => errorProvider.AddError($"Wrong instruction body '{instruction}' at line {line}.");

  public static void UndefinedFont(this IErrorProvider errorProvider, string fontAlias, int line)
    => errorProvider.AddError($"Undefined font '{fontAlias}' at line {line}.");

  public static void TableHasNotColumns(this IErrorProvider errorProvider, int line)
    => errorProvider.AddError($"Table has not columns at line {line}.");

  public static void TableHasNotRows(this IErrorProvider errorProvider, int line)
    => errorProvider.AddError($"Table has not rows at line {line}.");

  public static void WrongFontSize(this IErrorProvider errorProvider, string instruction, int line)
    => errorProvider.AddError($"Wrong font size '{instruction}' at line {line}.");

  public static void UnsupportedInstruction(this IErrorProvider errorProvider, string instruction, int line)
    => errorProvider.AddError($"Unsupported instruction '{instruction}' at line ({line}).");
}