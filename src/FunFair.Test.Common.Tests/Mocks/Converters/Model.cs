using System;
using System.Diagnostics.CodeAnalysis;

namespace FunFair.Test.Common.Tests.Mocks.Converters
{
    public sealed class Model : IEquatable<Model>
    {
        public Model()
        {
        }

        public Model(ModelColor color)
        {
            this.Value = color;
        }

        internal ModelColor? Value { get; init; }

        public bool Equals(Model? other)
        {
            if (ReferenceEquals(objA: null, objB: other))
            {
                return false;
            }

            if (ReferenceEquals(this, objB: other))
            {
                return true;
            }

            return this.Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, objB: obj) || obj is Model other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Value != null
                ? this.Value.GetHashCode()
                : 0;
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
                    value = new Model(ModelColor.RED);

                    return true;
                case "BLUE":
                    value = new Model(ModelColor.BLUE);

                    return true;
                default:
                    value = null;

                    return false;
            }
        }
    }
}