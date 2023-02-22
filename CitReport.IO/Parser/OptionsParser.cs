namespace CitReport.IO;

internal class OptionsParser
{
  public IEnumerable<Option> Parse(string current, IErrorProvider errorProvider)
  {
    var result = new List<Option>();
    var tokenizer = new Tokenizer(current);

    foreach (var option in tokenizer.GetTokens().Chunk(2))
    {
      var optionName = option[0];

      if (option.Length < 2)
      {
        errorProvider.AddError($"Option '{optionName}' has not value.");
        break;
      }

      if (!Options.Map.TryGetValue(optionName.ToLower(), out var optionType))
      {
        errorProvider.AddError($"Unsupported option '{optionName}'.");
        optionType = typeof(Option);
      }

      var optionValue = Activator.CreateInstance(optionType) as Option;
      if (optionValue == null)
      {
        errorProvider.AddError($"Type '{optionType.FullName}' does not inherit Option type.");
      }
      else
      {
        optionValue.Name = optionName;
        optionValue.Value = option[2];
        result.Add(optionValue);
      }
    }

    return result;
  }
}
