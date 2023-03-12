using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace ControlsSandbox.Converters;

public class MillimetersValueConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    => value is Thickness thikness
      ? ConvertThikness(thikness, MeasurementConverter.MillimetersToPixels)
      : ConvertValue(value, targetType, MeasurementConverter.MillimetersToPixels);

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    => value is Thickness thikness
      ? ConvertThikness(thikness, MeasurementConverter.PixelsToMillimeters)
      : ConvertValue(value, targetType, MeasurementConverter.PixelsToMillimeters);

  private object ConvertThikness(Thickness thickness, Func<double, double> convert)
    => new Thickness(convert(thickness.Left), convert(thickness.Top), convert(thickness.Right), convert(thickness.Bottom));

  private object ConvertValue(object value, Type targetType, Func<double, double> convert)
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

    var result = convert(millimeters);

    if (targetType == typeof(double))
    {
      return result;
    }
    else if (targetType == typeof(float))
    {
      return (float)result;
    }
    else if (targetType == typeof(int))
    {
      return (int)result;
    }
    else
    {
      throw new NotImplementedException();
    }
  }
}