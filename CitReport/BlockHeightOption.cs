namespace CitReport
{
  public class BlockHeightOption : Option
  {
    public override string Value
    {
      get => Height.ToString();
      set => Height = float.Parse(value);
    }

    public float Height { get; set; }
  }
}