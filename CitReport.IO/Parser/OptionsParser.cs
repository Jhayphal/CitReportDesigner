using System.ComponentModel;
using System.Reflection;

namespace CitReport.IO.Parser;

public class OptionsParser
{
  private static readonly Dictionary<string, Type> options = Assembly.GetAssembly(typeof(Option)).GetTypes()
    .Where(t => t != typeof(Option) && typeof(Option).IsAssignableFrom(t))
    .Select(t => new 
    { 
      Name = t.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName?.ToLower(), 
      Type = t 
    })
    .Where(x => !string.IsNullOrWhiteSpace(x.Name))
    .ToDictionary(x => x.Name, x => x.Type);

  private readonly OptionsTokenizer tokenizer = new();

  public IEnumerable<Option> Parse(ParserContext context, string current)
  {
    var result = new List<Option>();

    foreach (var option in tokenizer.GetTokens(current).Chunk(2))
    {
      var optionName = option[0];

      if (option.Length < 2)
      {
        context.ErrorProvider.OptionHasNotValue(optionName, context.CurrentLine);
        break;
      }

      if (!options.TryGetValue(optionName.ToLower(), out var optionType))
      {
        context.ErrorProvider.UnsupportedOption(optionName, context.CurrentLine);
        optionType = typeof(Option);
      }

      var optionValue = Activator.CreateInstance(optionType) as Option;
      if (optionValue == null)
      {
        context.ErrorProvider.WrongType(optionType, context.CurrentLine);
      }
      else
      {
        optionValue.Name = optionName;
        optionValue.Value = option[1];
        result.Add(optionValue);
      }
    }

    return result;
  }
}
