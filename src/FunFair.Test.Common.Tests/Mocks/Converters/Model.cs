using System;
using System.Diagnostics.CodeAnalysis;
using FunFair.Test.Infrastructure.Helpers;

namespace FunFair.Test.Common.Tests.Mocks.Converters;

public sealed class Model : IEquatable<Model>
{
    public Model() { }

    public Model(ModelColor color)
    {
        this.Value = color;
    }

    internal ModelColor? Value { get; init; }

    public bool Equals(Model? other)
    {
        return ReferenceObjectHelpers.AreEqual(left: this, right: other, eq: (l, r) => l.Value == r.Value);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, objB: obj))
        {
            return true;
        }

        return obj is Model other && this.Equals(other);
    }

    public override int GetHashCode()
    {
        return this.Value is not null ? this.Value.GetHashCode() : 0;
    }

    public static bool operator ==(Model left, Model right)
    {
        return Equals(objA: left, objB: right);
    }

    public static bool operator !=(Model left, Model right)
    {
        return !Equals(objA: left, objB: right);
    }

    public static bool TryParse(string source, [NotNullWhen(returnValue: true)] out Model? value)
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
                value = null;

                return false;
        }
    }
}
