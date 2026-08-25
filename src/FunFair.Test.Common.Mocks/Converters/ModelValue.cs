using System;

namespace FunFair.Test.Common.Mocks.Converters;

public readonly struct ModelValue : IEquatable<ModelValue>
{
    public ModelValue(ModelColor color)
    {
        this.Value = color;
    }

    public ModelColor Value { get; init; }

    public bool Equals(ModelValue other)
    {
        return this.Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is ModelValue other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return this.Value.GetHashCode();
    }

    public static bool operator ==(in ModelValue left, in ModelValue right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(in ModelValue left, in ModelValue right)
    {
        return !left.Equals(right);
    }

    public static bool TryParse(string source, out ModelValue value)
    {
        switch (source)
        {
            case "RED":
                value = new(ModelColor.RED);

                return true;
            case "BLUE":
                value = new(ModelColor.BLUE);

                return true;
            default:
                value = default;

                return false;
        }
    }
}
