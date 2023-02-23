﻿namespace CitReport;

public sealed class Metadata : IEquatable<Metadata>
{
  public string Value { get; set; }

  public bool Equals(Metadata other) => other != null && Value == other.Value;

  public override bool Equals(object obj) => Equals(obj as Metadata);

  public override int GetHashCode() => Value.GetHashCode();

  public override string ToString() => Value;
}