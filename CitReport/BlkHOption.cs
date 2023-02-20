namespace CitReport
{
  public static class Options
  {
    public const string BlkH = "blkh";
    public const string New_Linebreak = "new_linebreak";
    public const string Cond = "cond";
    public const string TCond = "tcond";
    public const string Uf = "uf";
    public const string Details = "details";
    public const string Break = "break";
    public const string Ctl = "ctl";
    public const string Skipper = "skipper";
    public const string Totals = "totals";

    public static readonly Dictionary<string, Type> Map = new Dictionary<string, Type>
    {
      { BlkH, typeof(BlkHOption) },
      { New_Linebreak, typeof(New_LinebreakOption) },
      { Cond, typeof(CondOption) },
      { TCond, typeof(TCondOption) },
      { Uf, typeof(UfOption) },
      { Details, typeof(DetailsOption) },
      { Break, typeof(BreakOption) },
      { Ctl, typeof(CtlOption) },
      { Skipper, typeof(SkipperOption) },
      { Totals, typeof(TotalsOption) }
    };
  }

  public interface IErrorProvider
  {
    void AddError(string message);
  }

  public class BlkHOption : Option
  {
    public override string Value
    {
      get => Height.ToString();
      set => Height = float.Parse(value);
    }

    public float Height { get; set; }
  }

  public class New_LinebreakOption : Option { }

  public class CondOption : Option { }

  public class TCondOption : Option { }

  public class UfOption : Option { }

  public class DetailsOption : Option { }

  public class BreakOption : Option { }

  public class CtlOption : Option { }

  public class SkipperOption : Option { }

  public class TotalsOption : Option { }
}