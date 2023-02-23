namespace CitReport.IO.Parser.Tests
{
  internal class TestErrorProvider : IErrorProvider
  {
    public readonly List<string> Errors = new();

    public void AddError(string message) => Errors.Add(message);
  }
}