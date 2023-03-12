using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace ControlsSandbox.Converters
{
  public class MillimetersValueConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double millimeters = 0d;

      if (value is double d)
      {
        millimeters = d;
      }
      else if (value is float f)
      {
        millimeters = f;
      }
      else if (value is int i)
      {
        millimeters = i;
      }

      return MeasurementConverter.MillimetersToPixels(millimeters);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      double pixels = 0d;

      if (value is double d)
      {
        pixels = d;
      }
      else if (value is float f)
      {
        pixels = f;
      }
      else if (value is int i)
      {
        pixels = i;
      }

      return MeasurementConverter.PixelsToMillimeters(pixels);
    }
  }
}