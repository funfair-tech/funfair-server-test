using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FunFair.Test.Common.Helpers;

namespace FunFair.Test.Common.Tests
{
    [SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global", Justification = "Base class for further tests")]
    public sealed class ComparableObjectTests : ComparableObjectTestBase<string>
    {
        public ComparableObjectTests()
            : base(value1: "Hello",
                   equivalentToValue1: new("olleH".Reverse()
                                                  .ToArray()),
                   value2: "World",
                   zeroObject: string.Empty)
        {
        }

        protected override bool OperatorEquals(string? x, string? y)
        {
            return ReferenceObjectHelpers.AreEqual(left: x, right: y, eq: (left, right) => StringComparer.Ordinal.Equals(x: left, y: right));
        }

        protected override bool OperatorNotEquals(string? x, string? y)
        {
            return !ReferenceObjectHelpers.AreEqual(left: x, right: y, eq: (left, right) => StringComparer.Ordinal.Equals(x: left, y: right));
        }

        protected override bool OperatorGreaterThanOrEqualTo(string? l, string? r)
        {
            return ReferenceObjectHelpers.Compare(left: l, right: r, cmp: (left, right) => StringComparer.Ordinal.Compare(x: left, y: right)) >= 0;
        }

        protected override bool OperatorLessThanOrEqualTo(string? l, string? r)
        {
            return ReferenceObjectHelpers.Compare(left: l, right: r, cmp: (left, right) => StringComparer.Ordinal.Compare(x: left, y: right)) <= 0;
        }

        protected override bool OperatorGreaterThan(string? l, string? r)
        {
            return ReferenceObjectHelpers.Compare(left: l, right: r, cmp: (left, right) => StringComparer.Ordinal.Compare(x: left, y: right)) > 0;
        }

        protected override bool OperatorLessThan(string? l, string? r)
        {
            return ReferenceObjectHelpers.Compare(left: l, right: r, cmp: (left, right) => StringComparer.Ordinal.Compare(x: left, y: right)) < 0;
        }
    }
}