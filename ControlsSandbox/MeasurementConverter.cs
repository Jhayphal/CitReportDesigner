using System.Runtime.InteropServices;
using System;
using Avalonia.Win32;
using Avalonia;
using Avalonia.Platform;
using Avalonia.Skia;

namespace ControlsSandbox;

public static class MeasurementConverter
{
  private enum MONITOR
  {
    MONITOR_DEFAULTTONULL = 0x00000000,
    MONITOR_DEFAULTTOPRIMARY = 0x00000001,
    MONITOR_DEFAULTTONEAREST = 0x00000002,
  }

  private enum MONITOR_DPI_TYPE
  {
    MDT_EFFECTIVE_DPI = 0,
    MDT_ANGULAR_DPI = 1,
    MDT_RAW_DPI = 2,
    MDT_DEFAULT = MDT_EFFECTIVE_DPI
  }

  private const double MillimetersInInch = 25.4d;

  private static double DPI = 96d;

  private static double ScaleFactor = 1d;

  public static void InitializeFrom(IWindowImpl window)
  {
    DPI = GetCurrentDpi(window).X;
    ScaleFactor = window.RenderScaling;
  }

  public static double MillimetersToPixels(double value) => value / MillimetersInInch * DPI / ScaleFactor;

  public static double PixelsToMillimeters(double value) => value * MillimetersInInch / DPI * ScaleFactor;

  private static Vector GetCurrentDpi(IWindowImpl window)
  {
    var shCoreAvailable = LoadLibrary("shcore.dll") != IntPtr.Zero;
    if (shCoreAvailable && Win32Platform.WindowsVersion > PlatformConstants.Windows8)
    {
      var monitor = MonitorFromWindow(window.Handle.Handle, MONITOR.MONITOR_DEFAULTTONEAREST);

      if (GetDpiForMonitor(monitor, MONITOR_DPI_TYPE.MDT_RAW_DPI, out var dpiX, out var dpiY) == 0)
      {
        return new Vector(dpiX, dpiY);
      }
    }

    return SkiaPlatform.DefaultDpi;
  }

  [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
  private static extern IntPtr LoadLibrary(string fileName);

  [DllImport("user32.dll")]
  private static extern IntPtr MonitorFromWindow(IntPtr hwnd, MONITOR dwFlags);

  [DllImport("shcore.dll")]
  private static extern long GetDpiForMonitor(IntPtr hmonitor, MONITOR_DPI_TYPE dpiType, out uint dpiX, out uint dpiY);
}
