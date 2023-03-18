namespace ControlsSandbox;

public readonly struct MillimetersToPixelsBoundsAdapter : IBounds
{
  private readonly IBounds m_adapter;

  public MillimetersToPixelsBoundsAdapter(IBounds draggable)
  {
    m_adapter = draggable;
  }

  public double X
  {
    get => MeasurementConverter.MillimetersToPixels(m_adapter.X);
    set => m_adapter.X = MeasurementConverter.PixelsToMillimeters(value);
  }

  public double Y
  {
    get => MeasurementConverter.MillimetersToPixels(m_adapter.Y);
    set => m_adapter.Y = MeasurementConverter.PixelsToMillimeters(value);
  }

  public double Width
  {
    get => MeasurementConverter.MillimetersToPixels(m_adapter.Width);
    set => m_adapter.Width = MeasurementConverter.PixelsToMillimeters(value);
  }

  public double Height
  {
    get => MeasurementConverter.MillimetersToPixels(m_adapter.Height);
    set => m_adapter.Height = MeasurementConverter.PixelsToMillimeters(value);
  }

  public ReportSizeUnit SizeUnit => ReportSizeUnit.Millimeter;
}