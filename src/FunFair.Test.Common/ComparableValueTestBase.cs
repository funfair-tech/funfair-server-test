using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Xunit;

namespace FunFair.Test.Common;

public abstract class ComparableValueTestBase<TObject> : EquatableValueTestBase<TObject>
    where TObject : struct, IEquatable<TObject>, IComparable<TObject>, IComparable
{
    protected ComparableValueTestBase(TObject zeroObject, TObject value1, TObject equivalentToValue1, TObject value2)
        : base(zeroObject: zeroObject, value1: value1, equivalentToValue1: equivalentToValue1)
    {
        this.Value2 = value2;
    }

    private TObject Value2 { get; }

    protected abstract bool OperatorGreaterThanOrEqualTo(in TObject l, in TObject r);

    protected abstract bool OperatorLessThanOrEqualTo(in TObject l, in TObject r);

    protected abstract bool OperatorGreaterThan(in TObject l, in TObject r);

    protected abstract bool OperatorLessThan(in TObject l, in TObject r);

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int TypedCompareTo(in TObject l, in TObject r)
    {
        return DoTypedCompareTo(l: l, r: r);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DoTypedCompareTo<T>(in T l, in T r)
        where T : struct, IComparable<T>
    {
        return l.CompareTo(r);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int DoUntypedCompareTo<T>(in T l, object? r)
        where T : struct, IComparable
    {
        return l.CompareTo(r);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int UntypedCompareTo(in TObject l, object? r)
    {
        return DoUntypedCompareTo(l: l, r: r);
    }

    [Fact]
    public void OperatorGreaterThanOrEqualToValue1IsGreaterThanOrEquivalentToValue1()
    {
        Assert.True(this.OperatorGreaterThanOrEqualTo(l: this.Value1, r: this.EquivalentToValue1), userMessage: "Value1 >= Value1");
    }

    [Fact]
    public void OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToValue1()
    {
        Assert.True(this.OperatorGreaterThanOrEqualTo(l: this.Value1, r: this.Value1), userMessage: "Value1 >= Value1");
    }

    [Fact]
    public void OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToValue2()
    {
        Assert.False(this.OperatorGreaterThanOrEqualTo(l: this.Value1, r: this.Value2), userMessage: "Value1 >= Value2");
    }

    [Fact]
    public void OperatorGreaterThanOrEqualToValue2IsGreaterThanOrEquivalentToValue1()
    {
        Assert.True(this.OperatorGreaterThanOrEqualTo(l: this.Value2, r: this.Value1), userMessage: "Value2 >= Value1");
    }

    [Fact]
    public void OperatorGreaterThanValue1IsNotGreaterThanValue1()
    {
        Assert.False(this.OperatorGreaterThan(l: this.Value1, r: this.Value1), userMessage: "Value1 > Value1");
    }

    [Fact]
    public void OperatorGreaterThanValue1IsNotGreaterThanValue2()
    {
        Assert.False(this.OperatorGreaterThan(l: this.Value1, r: this.Value2), userMessage: "Value1 > Value2");
    }

    [Fact]
    public void OperatorGreaterThanValue2IsGreaterThanValue1()
    {
        Assert.True(this.OperatorGreaterThan(l: this.Value2, r: this.Value1), userMessage: "Value2 > Value1");
    }

    [Fact]
    public void OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToValue1()
    {
        Assert.True(this.OperatorLessThanOrEqualTo(l: this.Value1, r: this.EquivalentToValue1), userMessage: "Value1 <= Value1");
    }

    [Fact]
    public void OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToValue2()
    {
        Assert.True(this.OperatorLessThanOrEqualTo(l: this.Value1, r: this.Value2), userMessage: "Value1 <= Value2");
    }

    [Fact]
    public void OperatorLessThanOrEqualToValue1IsNotLessThanOrEquivalentToValue1()
    {
        Assert.True(this.OperatorLessThanOrEqualTo(l: this.Value1, r: this.Value1), userMessage: "Value1 <= Value1");
    }

    [Fact]
    public void OperatorLessThanOrEqualToValue2IsNotLessThanOrEquivalentToValue1()
    {
        Assert.False(this.OperatorLessThanOrEqualTo(l: this.Value2, r: this.Value1), userMessage: "Value2 <= Value1");
    }

    [Fact]
    public void OperatorLessThanValue1IsLessThanValue2()
    {
        Assert.True(this.OperatorLessThan(l: this.Value1, r: this.Value2), userMessage: "Value1 < Value2");
    }

    [Fact]
    public void OperatorLessThanValue1IsNotLessThanValue1()
    {
        Assert.False(this.OperatorLessThan(l: this.Value1, r: this.Value1), userMessage: "Value1 < Value1");
    }

    [Fact]
    public void OperatorLessThanValue2IsNotLessThanValue1()
    {
        Assert.False(this.OperatorLessThan(l: this.Value2, r: this.Value1), userMessage: "Value2 < Value1");
    }

    [Fact]
    public void TypedCompareToValue1EqualToEquivalentToValue1()
    {
        Assert.True(TypedCompareTo(l: this.Value1, r: this.EquivalentToValue1) == 0, userMessage: "Should be equal to 0");
    }

    [Fact]
    public void TypedCompareToValue1LessThanValue2()
    {
        Assert.True(TypedCompareTo(l: this.Value1, r: this.Value2) < 0, userMessage: "Should be less than 0");
    }

    [Fact]
    public void TypedCompareToValue2GreaterThanValue1()
    {
        Assert.True(TypedCompareTo(l: this.Value2, r: this.Value1) > 0, userMessage: "Should be greater than 0");
    }

    [Fact]
    public void UntypedCompareToValue1EqualsUnTypedValue1Alias()
    {
        Assert.True(UntypedCompareTo(l: this.Value1, r: this.EquivalentToValue1AsObject) == 0, userMessage: "Should be equal to 0");
    }

    [Fact]
    public void UntypedCompareToValue1EqualToEquivalentToValue1()
    {
        Assert.True(UntypedCompareTo(l: this.Value1, r: this.EquivalentToValue1) == 0, userMessage: "Should be equal to 0");
    }

    [Fact]
    public void UntypedCompareToValue1LessThanValue2()
    {
        Assert.True(UntypedCompareTo(l: this.Value1, r: this.Value2) < 0, userMessage: "Should be less than 0");
    }

    [Fact]
    public void UntypedCompareToValue1ToOtherTypedObjectThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(testCode: () => UntypedCompareTo(l: this.Value1, Guid.NewGuid()));
    }

    [Fact]
    public void UntypedCompareToValue2GreaterThanUnTypedValue1Alias()
    {
        Assert.True(UntypedCompareTo(l: this.Value2, r: this.EquivalentToValue1AsObject) > 0, userMessage: "Should be greater than to 0");
    }

    [Fact]
    public void UntypedCompareToValue2GreaterThanValue1()
    {
        Assert.True(UntypedCompareTo(l: this.Value2, r: this.Value1) > 0, userMessage: "Should be greater than 0");
    }
}
