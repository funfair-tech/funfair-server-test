using System;

namespace FunFair.Test.Common.Tests.Mocks.JsonConverter
{
    public sealed class Model : IEquatable<Model>
    {
        internal ModelColor? Value { get; set; }

        public bool Equals(Model other)
        {
            if (ReferenceEquals(objA: null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is Model other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Value != null ? this.Value.GetHashCode() : 0;
        }

        public static bool operator ==(Model left, Model right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Model left, Model right)
        {
            return !Equals(left, right);
        }
    }
}