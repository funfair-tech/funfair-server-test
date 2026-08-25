using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FunFair.Test.Common;
using FunFair.Test.Infrastructure.Helpers;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class ComparableObjectTests : ComparableObjectTestBase<string>
{
    public ComparableObjectTests()
        : base(zeroObject: string.Empty, value1: "Hello", new([.. "olleH".Reverse()]), value2: "World") { }

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

    [SuppressMessage(
        category: "Meziantou.Analyzer",
        checkId: "MA0051:Method is too long",
        Justification = "Flat data table of AOT test cases, not control-flow complexity"
    )]
    public static TheoryData<string, Action<ComparableObjectTests>> BaseCaseData() =>
        new()
        {
            // EquatableObjectTestBase<T>
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
            // ComparableObjectTestBase<T>
            {
                nameof(OperatorGreaterOrEqualToThanNullObjectIsNotGreaterOrEquivalentToNullObject),
                t => t.OperatorGreaterOrEqualToThanNullObjectIsNotGreaterOrEquivalentToNullObject()
            },
            {
                nameof(OperatorGreaterThanNullObjectIsGreaterThanValue1),
                t => t.OperatorGreaterThanNullObjectIsGreaterThanValue1()
            },
            {
                nameof(OperatorGreaterThanNullObjectIsNotGreaterThanNullObject),
                t => t.OperatorGreaterThanNullObjectIsNotGreaterThanNullObject()
            },
            {
                nameof(OperatorGreaterThanOrEqualToNullObjectIsGreaterThanOrEquivalentToValue1),
                t => t.OperatorGreaterThanOrEqualToNullObjectIsGreaterThanOrEquivalentToValue1()
            },
            {
                nameof(OperatorGreaterThanOrEqualToValue1IsGreaterThanOrEquivalentToValue1),
                t => t.OperatorGreaterThanOrEqualToValue1IsGreaterThanOrEquivalentToValue1()
            },
            {
                nameof(OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToNullObject),
                t => t.OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToNullObject()
            },
            {
                nameof(OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToValue1),
                t => t.OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToValue1()
            },
            {
                nameof(OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToValue2),
                t => t.OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToValue2()
            },
            {
                nameof(OperatorGreaterThanOrEqualToValue2IsGreaterThanOrEquivalentToValue1),
                t => t.OperatorGreaterThanOrEqualToValue2IsGreaterThanOrEquivalentToValue1()
            },
            {
                nameof(OperatorGreaterThanOrEqualToValue2IsNotGreaterThanOrEquivalentToNullObject),
                t => t.OperatorGreaterThanOrEqualToValue2IsNotGreaterThanOrEquivalentToNullObject()
            },
            {
                nameof(OperatorGreaterThanValue1IsNotGreaterThanNullObject),
                t => t.OperatorGreaterThanValue1IsNotGreaterThanNullObject()
            },
            {
                nameof(OperatorGreaterThanValue1IsNotGreaterThanValue1),
                t => t.OperatorGreaterThanValue1IsNotGreaterThanValue1()
            },
            {
                nameof(OperatorGreaterThanValue1IsNotGreaterThanValue2),
                t => t.OperatorGreaterThanValue1IsNotGreaterThanValue2()
            },
            {
                nameof(OperatorGreaterThanValue2IsGreaterThanValue1),
                t => t.OperatorGreaterThanValue2IsGreaterThanValue1()
            },
            {
                nameof(OperatorGreaterThanValue2IsNotGreaterThanNullObject),
                t => t.OperatorGreaterThanValue2IsNotGreaterThanNullObject()
            },
            {
                nameof(OperatorLessOrEqualToThanNullObjectIsLessThanOrEquivalentToNullObject),
                t => t.OperatorLessOrEqualToThanNullObjectIsLessThanOrEquivalentToNullObject()
            },
            {
                nameof(OperatorLessThanNullObjectIsLessThanNullObject),
                t => t.OperatorLessThanNullObjectIsLessThanNullObject()
            },
            {
                nameof(OperatorLessThanNullObjectIsNotLessThanValue1),
                t => t.OperatorLessThanNullObjectIsNotLessThanValue1()
            },
            {
                nameof(OperatorLessThanOrEqualToNullObjectIsNotLessThanOrEquivalentToValue1),
                t => t.OperatorLessThanOrEqualToNullObjectIsNotLessThanOrEquivalentToValue1()
            },
            {
                nameof(OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToNullObject),
                t => t.OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToNullObject()
            },
            {
                nameof(OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToValue1),
                t => t.OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToValue1()
            },
            {
                nameof(OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToValue2),
                t => t.OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToValue2()
            },
            {
                nameof(OperatorLessThanOrEqualToValue1IsNotLessThanOrEquivalentToValue1),
                t => t.OperatorLessThanOrEqualToValue1IsNotLessThanOrEquivalentToValue1()
            },
            {
                nameof(OperatorLessThanOrEqualToValue2IsLessThanOrEquivalentToNullObject),
                t => t.OperatorLessThanOrEqualToValue2IsLessThanOrEquivalentToNullObject()
            },
            {
                nameof(OperatorLessThanOrEqualToValue2IsNotLessThanOrEquivalentToValue1),
                t => t.OperatorLessThanOrEqualToValue2IsNotLessThanOrEquivalentToValue1()
            },
            { nameof(OperatorLessThanValue1IsLessThanNullObject), t => t.OperatorLessThanValue1IsLessThanNullObject() },
            { nameof(OperatorLessThanValue1IsLessThanValue2), t => t.OperatorLessThanValue1IsLessThanValue2() },
            { nameof(OperatorLessThanValue1IsNotLessThanValue1), t => t.OperatorLessThanValue1IsNotLessThanValue1() },
            { nameof(OperatorLessThanValue2IsLessThanNullObject), t => t.OperatorLessThanValue2IsLessThanNullObject() },
            { nameof(OperatorLessThanValue2IsNotLessThanValue1), t => t.OperatorLessThanValue2IsNotLessThanValue1() },
            {
                nameof(TypedCompareToValue1EqualToEquivalentToValue1),
                t => t.TypedCompareToValue1EqualToEquivalentToValue1()
            },
            { nameof(TypedCompareToValue1GreaterThanNullObject), t => t.TypedCompareToValue1GreaterThanNullObject() },
            { nameof(TypedCompareToValue1LessThanValue2), t => t.TypedCompareToValue1LessThanValue2() },
            { nameof(TypedCompareToValue2GreaterThanValue1), t => t.TypedCompareToValue2GreaterThanValue1() },
            {
                nameof(UntypedCompareToValue1EqualsUnTypedValue1Alias),
                t => t.UntypedCompareToValue1EqualsUnTypedValue1Alias()
            },
            {
                nameof(UntypedCompareToValue1EqualToEquivalentToValue1),
                t => t.UntypedCompareToValue1EqualToEquivalentToValue1()
            },
            {
                nameof(UntypedCompareToValue1GreaterThanToNullObject),
                t => t.UntypedCompareToValue1GreaterThanToNullObject()
            },
            { nameof(UntypedCompareToValue1LessThanValue2), t => t.UntypedCompareToValue1LessThanValue2() },
            {
                nameof(UntypedCompareToValue1ToOtherTypedObjectThrowsArgumentException),
                t => t.UntypedCompareToValue1ToOtherTypedObjectThrowsArgumentException()
            },
            {
                nameof(UntypedCompareToValue2GreaterThanUnTypedValue1Alias),
                t => t.UntypedCompareToValue2GreaterThanUnTypedValue1Alias()
            },
            { nameof(UntypedCompareToValue2GreaterThanValue1), t => t.UntypedCompareToValue2GreaterThanValue1() },
        };

    [Theory]
    [MemberData(nameof(BaseCaseData))]
    public void CommonTests(string name, Action<ComparableObjectTests> action)
    {
        Assert.NotEmpty(name);
        action(this);
    }
}
