var folder = @"D:\VOPRJ8sp3\WinZ_16070R\BIN\REP";

void ShowAll(string startsWith, string alias)
{
  var logFileName = @$"D:\Logs\{alias}.txt";

  if (File.Exists(logFileName))
  {
    foreach (var line in File.ReadAllLines(logFileName))
    {
      Console.WriteLine(line);
    }

    return;
  }

  var instructions = new HashSet<string>();
  foreach (var file in Directory.EnumerateFiles(folder!, "*.rep"))
  {
    foreach (var line in File.ReadAllLines(file))
    {
      if (line.StartsWith(startsWith))
      {
        instructions.Add(line);
      }
    }
  }

  var log = File.CreateText(logFileName);
  foreach (var instruction in instructions)
  {
    log.WriteLine(instruction);
    Console.WriteLine(instruction);
  }
}

ShowAll("{/FL,", "FL");

Console.ReadLine();