using Avalonia.Controls;
using System;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Win32;

namespace ControlsSandbox.Views
{
  public partial class MainWindow : Window
  {
    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern IntPtr LoadLibrary(string fileName);

    [DllImport("user32.dll")]
    public static extern IntPtr MonitorFromWindow(IntPtr hwnd, MONITOR dwFlags);

    [DllImport("shcore.dll")]
    public static extern long GetDpiForMonitor(IntPtr hmonitor, MONITOR_DPI_TYPE dpiType, out uint dpiX, out uint dpiY);

    public enum MONITOR
    {
      MONITOR_DEFAULTTONULL = 0x00000000,
      MONITOR_DEFAULTTOPRIMARY = 0x00000001,
      MONITOR_DEFAULTTONEAREST = 0x00000002,
    }

    public enum MONITOR_DPI_TYPE
    {
      MDT_EFFECTIVE_DPI = 0,
      MDT_ANGULAR_DPI = 1,
      MDT_RAW_DPI = 2,
      MDT_DEFAULT = MDT_EFFECTIVE_DPI
    }

    public MainWindow()
    {
      InitializeComponent();

      var dpi = GetCurrentDpi();
      MeasurementConverter.DPI = dpi.X;
      MeasurementConverter.ScaleFactor = PlatformImpl.RenderScaling;
    }

    public static bool ShCoreAvailable => LoadLibrary("shcore.dll") != IntPtr.Zero;

    private Vector GetCurrentDpi()
    {
      if (ShCoreAvailable && Win32Platform.WindowsVersion > PlatformConstants.Windows8)
      {
        var monitor = MonitorFromWindow(PlatformImpl.Handle.Handle, MONITOR.MONITOR_DEFAULTTONEAREST);

        if (GetDpiForMonitor(
            monitor,
            MONITOR_DPI_TYPE.MDT_RAW_DPI,
            out var dpiX,
            out var dpiY) == 0)
        {
          return new Vector(dpiX, dpiY);
        }
      }

      return new Vector(96, 96);
    }
  }
}
