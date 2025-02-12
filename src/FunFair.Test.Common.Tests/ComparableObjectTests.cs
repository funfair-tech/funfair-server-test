using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FunFair.Test.Common.Helpers;

namespace FunFair.Test.Common.Tests;

[SuppressMessage(category: "ReSharper", checkId: "UnusedType.Global", Justification = "Base class for further tests")]
public sealed class ComparableObjectTests : ComparableObjectTestBase<string>
{
    public ComparableObjectTests()
        : base(zeroObject: string.Empty,
               value1: "Hello",
               new([
                   .."olleH".Reverse()
               ]),
               value2: "World")
    {
    }

    protected override bool OperatorEquals(string? x, string? y)
    {
        return ReferenceObjectHelpers.AreEqual(left: x, right: y, eq: StringComparer.Ordinal.Equals);
    }

    protected override bool OperatorNotEquals(string? x, string? y)
    {
        return !ReferenceObjectHelpers.AreEqual(left: x, right: y, eq: StringComparer.Ordinal.Equals);
    }

    protected override bool OperatorGreaterThanOrEqualTo(string? l, string? r)
    {
        return ReferenceObjectHelpers.Compare(left: l, right: r, cmp: StringComparer.Ordinal.Compare) >= 0;
    }

    protected override bool OperatorLessThanOrEqualTo(string? l, string? r)
    {
        return ReferenceObjectHelpers.Compare(left: l, right: r, cmp: StringComparer.Ordinal.Compare) <= 0;
    }

    protected override bool OperatorGreaterThan(string? l, string? r)
    {
        return ReferenceObjectHelpers.Compare(left: l, right: r, cmp: StringComparer.Ordinal.Compare) > 0;
    }

    protected override bool OperatorLessThan(string? l, string? r)
    {
        return ReferenceObjectHelpers.Compare(left: l, right: r, cmp: StringComparer.Ordinal.Compare) < 0;
    }
}