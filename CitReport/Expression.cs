namespace CitReport;

public class Expression : IEquatable<Expression>
{
  public string Value { get; set; }

  public bool Equals(Expression other) => other is not null && Value == other.Value;

  public override bool Equals(object obj) => Equals(obj as Expression);

  public override int GetHashCode() => Value.GetHashCode();

  public override string ToString() => Value;

  public static bool operator ==(Expression left, Expression right) => left is null
    ? right is null
    : left.Equals(right);

  public static bool operator !=(Expression left, Expression right) => !(left == right);
}