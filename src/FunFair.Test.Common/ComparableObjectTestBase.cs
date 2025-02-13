using System;
using Xunit;

namespace FunFair.Test.Common;

public abstract class ComparableObjectTestBase<TObject> : EquatableObjectTestBase<TObject>
    where TObject : class, IEquatable<TObject>, IComparable<TObject>, IComparable
{
    protected ComparableObjectTestBase(
        TObject zeroObject,
        TObject value1,
        TObject equivalentToValue1,
        TObject value2
    )
        : base(zeroObject: zeroObject, value1: value1, equivalentToValue1: equivalentToValue1)
    {
        this.Value2 = value2;
    }

    private TObject Value2 { get; }

    protected abstract bool OperatorGreaterThanOrEqualTo(TObject? l, TObject? r);

    protected abstract bool OperatorLessThanOrEqualTo(TObject? l, TObject? r);

    protected abstract bool OperatorGreaterThan(TObject? l, TObject? r);

    protected abstract bool OperatorLessThan(TObject? l, TObject? r);

    private static int TypedCompareTo(TObject l, TObject? r)
    {
        IComparable<TObject> cmp = l;

        return cmp.CompareTo(r);
    }

    private static int UntypedCompareTo(TObject l, object? r)
    {
        IComparable cmp = l;

        return cmp.CompareTo(r);
    }

    [Fact]
    public void OperatorGreaterOrEqualToThanNullObjectIsNotGreaterOrEquivalentToNullObject()
    {
        Assert.False(
            this.OperatorGreaterThanOrEqualTo(l: this.Value1, r: this.NullObject),
            userMessage: "NullObject >= Value1"
        );
    }

    [Fact]
    public void OperatorGreaterThanNullObjectIsGreaterThanValue1()
    {
        Assert.True(
            this.OperatorGreaterThan(l: this.NullObject, r: this.Value2),
            userMessage: "NullObject > Value2"
        );
    }

    [Fact]
    public void OperatorGreaterThanNullObjectIsNotGreaterThanNullObject()
    {
        Assert.False(
            this.OperatorGreaterThan(l: this.Value1, r: this.NullObject),
            userMessage: "NullObject > Value1"
        );
    }

    [Fact]
    public void OperatorGreaterThanOrEqualToNullObjectIsGreaterThanOrEquivalentToValue1()
    {
        Assert.True(
            this.OperatorGreaterThanOrEqualTo(l: this.NullObject, r: this.Value2),
            userMessage: "NullObject >= Value2"
        );
    }

    [Fact]
    public void OperatorGreaterThanOrEqualToValue1IsGreaterThanOrEquivalentToValue1()
    {
        Assert.True(
            this.OperatorGreaterThanOrEqualTo(l: this.Value1, r: this.EquivalentToValue1),
            userMessage: "Value1 >= Value1"
        );
    }

    [Fact]
    public void OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToNullObject()
    {
        Assert.False(
            this.OperatorGreaterThanOrEqualTo(l: this.Value1, r: this.NullObject),
            userMessage: "Value1 >= NullObject"
        );
    }

    [Fact]
    public void OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToValue1()
    {
        Assert.True(
            this.OperatorGreaterThanOrEqualTo(l: this.Value1, r: this.Value1),
            userMessage: "Value1 >= Value1"
        );
    }

    [Fact]
    public void OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToValue2()
    {
        Assert.False(
            this.OperatorGreaterThanOrEqualTo(l: this.Value1, r: this.Value2),
            userMessage: "Value1 >= Value2"
        );
    }

    [Fact]
    public void OperatorGreaterThanOrEqualToValue2IsGreaterThanOrEquivalentToValue1()
    {
        Assert.True(
            this.OperatorGreaterThanOrEqualTo(l: this.Value2, r: this.Value1),
            userMessage: "Value2 >= Value1"
        );
    }

    [Fact]
    public void OperatorGreaterThanOrEqualToValue2IsNotGreaterThanOrEquivalentToNullObject()
    {
        Assert.False(
            this.OperatorGreaterThanOrEqualTo(l: this.Value2, r: this.NullObject),
            userMessage: "Value2 >= NullObject"
        );
    }

    [Fact]
    public void OperatorGreaterThanValue1IsNotGreaterThanNullObject()
    {
        Assert.False(
            this.OperatorGreaterThan(l: this.Value1, r: this.NullObject),
            userMessage: "Value1 > NullObject"
        );
    }

    [Fact]
    public void OperatorGreaterThanValue1IsNotGreaterThanValue1()
    {
        Assert.False(
            this.OperatorGreaterThan(l: this.Value1, r: this.Value1),
            userMessage: "Value1 > Value1"
        );
    }

    [Fact]
    public void OperatorGreaterThanValue1IsNotGreaterThanValue2()
    {
        Assert.False(
            this.OperatorGreaterThan(l: this.Value1, r: this.Value2),
            userMessage: "Value1 > Value2"
        );
    }

    [Fact]
    public void OperatorGreaterThanValue2IsGreaterThanValue1()
    {
        Assert.True(
            this.OperatorGreaterThan(l: this.Value2, r: this.Value1),
            userMessage: "Value2 > Value1"
        );
    }

    [Fact]
    public void OperatorGreaterThanValue2IsNotGreaterThanNullObject()
    {
        Assert.False(
            this.OperatorGreaterThan(l: this.Value2, r: this.NullObject),
            userMessage: "Value2 > NullObject"
        );
    }

    [Fact]
    public void OperatorLessOrEqualToThanNullObjectIsLessThanOrEquivalentToNullObject()
    {
        Assert.True(
            this.OperatorLessThanOrEqualTo(l: this.Value1, r: this.NullObject),
            userMessage: "NullObject <= Value1"
        );
    }

    [Fact]
    public void OperatorLessThanNullObjectIsLessThanNullObject()
    {
        Assert.True(
            this.OperatorLessThan(l: this.Value1, r: this.NullObject),
            userMessage: "NullObject < Value1"
        );
    }

    [Fact]
    public void OperatorLessThanNullObjectIsNotLessThanValue1()
    {
        Assert.False(
            this.OperatorLessThan(l: this.NullObject, r: this.Value2),
            userMessage: "NullObject < Value2"
        );
    }

    [Fact]
    public void OperatorLessThanOrEqualToNullObjectIsNotLessThanOrEquivalentToValue1()
    {
        Assert.False(
            this.OperatorLessThanOrEqualTo(l: this.NullObject, r: this.Value2),
            userMessage: "NullObject <= Value2"
        );
    }

    [Fact]
    public void OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToNullObject()
    {
        Assert.True(
            this.OperatorLessThanOrEqualTo(l: this.Value1, r: this.NullObject),
            userMessage: "Value1 <= NullObject"
        );
    }

    [Fact]
    public void OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToValue1()
    {
        Assert.True(
            this.OperatorLessThanOrEqualTo(l: this.Value1, r: this.EquivalentToValue1),
            userMessage: "Value1 <= Value1"
        );
    }

    [Fact]
    public void OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToValue2()
    {
        Assert.True(
            this.OperatorLessThanOrEqualTo(l: this.Value1, r: this.Value2),
            userMessage: "Value1 <= Value2"
        );
    }

    [Fact]
    public void OperatorLessThanOrEqualToValue1IsNotLessThanOrEquivalentToValue1()
    {
        Assert.True(
            this.OperatorLessThanOrEqualTo(l: this.Value1, r: this.Value1),
            userMessage: "Value1 <= Value1"
        );
    }

    [Fact]
    public void OperatorLessThanOrEqualToValue2IsLessThanOrEquivalentToNullObject()
    {
        Assert.True(
            this.OperatorLessThanOrEqualTo(l: this.Value2, r: this.NullObject),
            userMessage: "Value2 <= NullObject"
        );
    }

    [Fact]
    public void OperatorLessThanOrEqualToValue2IsNotLessThanOrEquivalentToValue1()
    {
        Assert.False(
            this.OperatorLessThanOrEqualTo(l: this.Value2, r: this.Value1),
            userMessage: "Value2 <= Value1"
        );
    }

    [Fact]
    public void OperatorLessThanValue1IsLessThanNullObject()
    {
        Assert.True(
            this.OperatorLessThan(l: this.Value1, r: this.NullObject),
            userMessage: "Value1 < NullObject"
        );
    }

    [Fact]
    public void OperatorLessThanValue1IsLessThanValue2()
    {
        Assert.True(
            this.OperatorLessThan(l: this.Value1, r: this.Value2),
            userMessage: "Value1 < Value2"
        );
    }

    [Fact]
    public void OperatorLessThanValue1IsNotLessThanValue1()
    {
        Assert.False(
            this.OperatorLessThan(l: this.Value1, r: this.Value1),
            userMessage: "Value1 < Value1"
        );
    }

    [Fact]
    public void OperatorLessThanValue2IsLessThanNullObject()
    {
        Assert.True(
            this.OperatorLessThan(l: this.Value2, r: this.NullObject),
            userMessage: "Value2 < NullObject"
        );
    }

    [Fact]
    public void OperatorLessThanValue2IsNotLessThanValue1()
    {
        Assert.False(
            this.OperatorLessThan(l: this.Value2, r: this.Value1),
            userMessage: "Value2 < Value1"
        );
    }

    [Fact]
    public void TypedCompareToValue1EqualToEquivalentToValue1()
    {
        Assert.True(
            TypedCompareTo(l: this.Value1, r: this.EquivalentToValue1) == 0,
            userMessage: "Should be equal to 0"
        );
    }

    [Fact]
    public void TypedCompareToValue1GreaterThanNullObject()
    {
        Assert.True(
            TypedCompareTo(l: this.Value1, r: this.NullObject) > 0,
            userMessage: "Should be greater than 0"
        );
    }

    [Fact]
    public void TypedCompareToValue1LessThanValue2()
    {
        Assert.True(
            TypedCompareTo(l: this.Value1, r: this.Value2) < 0,
            userMessage: "Should be less than 0"
        );
    }

    [Fact]
    public void TypedCompareToValue2GreaterThanValue1()
    {
        Assert.True(
            TypedCompareTo(l: this.Value2, r: this.Value1) > 0,
            userMessage: "Should be greater than 0"
        );
    }

    [Fact]
    public void UntypedCompareToValue1EqualsUnTypedValue1Alias()
    {
        Assert.True(
            UntypedCompareTo(l: this.Value1, r: this.EquivalentToValue1AsObject) == 0,
            userMessage: "Should be equal to 0"
        );
    }

    [Fact]
    public void UntypedCompareToValue1EqualToEquivalentToValue1()
    {
        Assert.True(
            UntypedCompareTo(l: this.Value1, r: this.EquivalentToValue1) == 0,
            userMessage: "Should be equal to 0"
        );
    }

    [Fact]
    public void UntypedCompareToValue1GreaterThanToNullObject()
    {
        Assert.True(
            UntypedCompareTo(l: this.Value1, r: this.NullObject) > 0,
            userMessage: "Should be greater than 0"
        );
    }

    [Fact]
    public void UntypedCompareToValue1LessThanValue2()
    {
        Assert.True(
            UntypedCompareTo(l: this.Value1, r: this.Value2) < 0,
            userMessage: "Should be less than 0"
        );
    }

    [Fact]
    public void UntypedCompareToValue1ToOtherTypedObjectThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(
            testCode: () => UntypedCompareTo(l: this.Value1, Guid.NewGuid())
        );
        UnusedVariable(exception);
    }

    [Fact]
    public void UntypedCompareToValue2GreaterThanUnTypedValue1Alias()
    {
        Assert.True(
            UntypedCompareTo(l: this.Value2, r: this.EquivalentToValue1AsObject) > 0,
            userMessage: "Should be greater than to 0"
        );
    }

    [Fact]
    public void UntypedCompareToValue2GreaterThanValue1()
    {
        Assert.True(
            UntypedCompareTo(l: this.Value2, r: this.Value1) > 0,
            userMessage: "Should be greater than 0"
        );
    }
}
