﻿using System.ComponentModel;
using System.Reflection;

namespace CitReport;

public class Option : IEquatable<Option>
{
  public Option()
  {
    Name = GetType().GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? string.Empty;
  }

  public string Name { get; set; }

  public virtual string Value { get; set; } = string.Empty;

  public override bool Equals(object obj) => Equals(obj as Option);

  public bool Equals(Option other)
    => other != null && other.GetType() == GetType() && other.Name == Name && other.Value == Value;

  public override int GetHashCode()
    => (GetType().Name + (Name ?? string.Empty) + (Value ?? string.Empty)).GetHashCode();

  public override string ToString() => $"{Name}({Value})";

  public static bool operator ==(Option left, Option right) => left is null
    ? right is null
    : left.Equals(right);

  public static bool operator !=(Option left, Option right) => !(left == right);
}

[DisplayName("BlkH")]
public class BlkHOption : Option
{
  public override string Value
  {
    get => Height.ToString();
    set => Height = double.Parse(value);
  }

  public double Height { get; set; }
}

[DisplayName("New_Linebreak")]
public class New_LinebreakOption : Option { }

[DisplayName("Cond")]
public class CondOption : Option { }

[DisplayName("TCond")]
public class TCondOption : Option { }

[DisplayName("UF")]
public class UfOption : Option { }

[DisplayName("Details")]
public class DetailsOption : Option { }

[DisplayName("Break")]
public class BreakOption : Option { }

[DisplayName("Ctl")]
public class CtlOption : Option { }

[DisplayName("Skipper")]
public class SkipperOption : Option { }

[DisplayName("Totals")]
public class TotalsOption : Option { }