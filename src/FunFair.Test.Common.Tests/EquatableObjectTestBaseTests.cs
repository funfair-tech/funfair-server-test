using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FunFair.Test.Common;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class EquatableObjectTestBaseTests : EquatableObjectTestBase<string>
{
    public EquatableObjectTestBaseTests()
        : base(zeroObject: string.Empty, value1: "Hello", new([.. "olleH".Reverse()])) { }

    protected override bool OperatorEquals(string? x, string? y)
    {
        return StringComparer.Ordinal.Equals(x: x, y: y);
    }

    protected override bool OperatorNotEquals(string? x, string? y)
    {
        return !StringComparer.Ordinal.Equals(x: x, y: y);
    }

    [Fact]
    public void Test()
    {
        Assert.Equal(expected: this.Value1, actual: this.Value1Alias);
    }

    [SuppressMessage(
        category: "Meziantou.Analyzer",
        checkId: "MA0051:Method is too long",
        Justification = "Flat data table of AOT test cases, not control-flow complexity"
    )]
    public static TheoryData<string, Action<EquatableObjectTestBaseTests>> BaseCaseData() =>
        new()
        {
            { nameof(GetHashCodeSameNoMatterHowManyTimesCalled), t => t.GetHashCodeSameNoMatterHowManyTimesCalled() },
            {
                nameof(GetHashCodeValue1ObjectIsSameAsEquivalentToValue1Object),
                t => t.GetHashCodeValue1ObjectIsSameAsEquivalentToValue1Object()
            },
            {
                nameof(GetHashCodeValue1ObjectIsSameAsValue1AliasObject),
                t => t.GetHashCodeValue1ObjectIsSameAsValue1AliasObject()
            },
            {
                nameof(GetHashCodeValue1ObjectIsSameAsValue1Object),
                t => t.GetHashCodeValue1ObjectIsSameAsValue1Object()
            },
            { nameof(GetHashCodeZeroObjectIsSameAsZeroObject), t => t.GetHashCodeZeroObjectIsSameAsZeroObject() },
            {
                nameof(OperatorEqualsNullObjectDifferentToZeroObject),
                t => t.OperatorEqualsNullObjectDifferentToZeroObject()
            },
            { nameof(OperatorEqualsNullObjectSameAsNullObject), t => t.OperatorEqualsNullObjectSameAsNullObject() },
            {
                nameof(OperatorEqualsValue1ObjectIsSameAsEquivalentToValue1Object),
                t => t.OperatorEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
            },
            {
                nameof(OperatorEqualsValue1ObjectIsSameAsValue1AliasObject),
                t => t.OperatorEqualsValue1ObjectIsSameAsValue1AliasObject()
            },
            {
                nameof(OperatorEqualsValue1ObjectIsSameAsValue1Object),
                t => t.OperatorEqualsValue1ObjectIsSameAsValue1Object()
            },
            {
                nameof(OperatorEqualsZeroObjectDifferentToNullObject),
                t => t.OperatorEqualsZeroObjectDifferentToNullObject()
            },
            { nameof(OperatorEqualsZeroObjectIsSameAsZeroObject), t => t.OperatorEqualsZeroObjectIsSameAsZeroObject() },
            {
                nameof(OperatorNotEqualsNullObjectDifferentToZeroObject),
                t => t.OperatorNotEqualsNullObjectDifferentToZeroObject()
            },
            {
                nameof(OperatorNotEqualsNullObjectSameAsNullObject),
                t => t.OperatorNotEqualsNullObjectSameAsNullObject()
            },
            {
                nameof(OperatorNotEqualsValue1ObjectIsSameAsEquivalentToValue1Object),
                t => t.OperatorNotEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
            },
            {
                nameof(OperatorNotEqualsValue1ObjectIsSameAsValue1AliasObject),
                t => t.OperatorNotEqualsValue1ObjectIsSameAsValue1AliasObject()
            },
            {
                nameof(OperatorNotEqualsValue1ObjectIsSameAsValue1Object),
                t => t.OperatorNotEqualsValue1ObjectIsSameAsValue1Object()
            },
            {
                nameof(OperatorNotEqualsZeroObjectDifferentToNullObject),
                t => t.OperatorNotEqualsZeroObjectDifferentToNullObject()
            },
            {
                nameof(OperatorNotEqualsZeroObjectIsSameAsZeroObject),
                t => t.OperatorNotEqualsZeroObjectIsSameAsZeroObject()
            },
            {
                nameof(TypedEqualsValue1ObjectIsSameAsEquivalentToValue1Object),
                t => t.TypedEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
            },
            {
                nameof(TypedEqualsValue1ObjectIsSameAsValue1AliasObject),
                t => t.TypedEqualsValue1ObjectIsSameAsValue1AliasObject()
            },
            {
                nameof(TypedEqualsValue1ObjectIsSameAsValue1Object),
                t => t.TypedEqualsValue1ObjectIsSameAsValue1Object()
            },
            { nameof(TypedEqualsZeroObjectDifferentToNullObject), t => t.TypedEqualsZeroObjectDifferentToNullObject() },
            { nameof(TypedEqualsZeroObjectIsSameAsZeroObject), t => t.TypedEqualsZeroObjectIsSameAsZeroObject() },
            {
                nameof(UntypedEqualsValue1ObjectIsSameAsEquivalentToValue1Object),
                t => t.UntypedEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
            },
            {
                nameof(UntypedEqualsValue1ObjectIsSameAsEquivalentToValue1ObjectAsObject),
                t => t.UntypedEqualsValue1ObjectIsSameAsEquivalentToValue1ObjectAsObject()
            },
            {
                nameof(UntypedEqualsValue1ObjectIsSameAsValue1AliasObject),
                t => t.UntypedEqualsValue1ObjectIsSameAsValue1AliasObject()
            },
            {
                nameof(UntypedEqualsValue1ObjectIsSameAsValue1Object),
                t => t.UntypedEqualsValue1ObjectIsSameAsValue1Object()
            },
            {
                nameof(UntypedEqualsZeroObjectDifferentToAnotherTypeOfObject),
                t => t.UntypedEqualsZeroObjectDifferentToAnotherTypeOfObject()
            },
            {
                nameof(UntypedEqualsZeroObjectDifferentToNullObject),
                t => t.UntypedEqualsZeroObjectDifferentToNullObject()
            },
            { nameof(UntypedEqualsZeroObjectIsSameAsZeroObject), t => t.UntypedEqualsZeroObjectIsSameAsZeroObject() },
        };

    [Theory]
    [MemberData(nameof(BaseCaseData))]
    public void CommonTests(string name, Action<EquatableObjectTestBaseTests> action)
    {
        Assert.NotEmpty(name);
        action(this);
    }
}
