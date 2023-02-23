using System.Diagnostics;

namespace CitReport.IO.Parser.Tests
{
  internal class DebugErrorProvider : IErrorProvider
  {
    public void AddError(string message) => Debug.WriteLine(message);
  }
}