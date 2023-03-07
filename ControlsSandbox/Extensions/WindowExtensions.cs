using Avalonia.Controls;
using Avalonia.Platform;
using System;
using System.Linq;

namespace ControlsSandbox.Extensions
{
  public static class WindowExtensions
  {
    public static Screen GetActiveScreen(this Window window)
    {
      return window?.Screens?.ScreenFromVisual(window) ?? GetPrimaryScreen(window);
    }

    public static Screen GetPrimaryScreen(this Window window)
    {
      var screens = window?.Screens;
      if (screens == null)
      {
        throw new Exception("Failed to read screens collection");
      }

      return screens.Primary
        ?? screens.All.FirstOrDefault()
        ?? throw new Exception("Screens collection is empty");
    }
  }
}
