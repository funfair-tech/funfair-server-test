using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using static FunFair.Test.Common.DispatcherCaseData;

namespace FunFair.Test.Common;

public abstract class ComparableObjectTestBase<TObject> : EquatableObjectTestBase<TObject>
    where TObject : class, IEquatable<TObject>, IComparable<TObject>, IComparable
{
    protected ComparableObjectTestBase(TObject zeroObject, TObject value1, TObject equivalentToValue1, TObject value2)
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
        Assert.True(this.OperatorGreaterThan(l: this.NullObject, r: this.Value2), userMessage: "NullObject > Value2");
    }

    [Fact]
    public void OperatorGreaterThanNullObjectIsNotGreaterThanNullObject()
    {
        Assert.False(this.OperatorGreaterThan(l: this.Value1, r: this.NullObject), userMessage: "NullObject > Value1");
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
        Assert.True(this.OperatorGreaterThanOrEqualTo(l: this.Value1, r: this.Value1), userMessage: "Value1 >= Value1");
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
        Assert.True(this.OperatorGreaterThanOrEqualTo(l: this.Value2, r: this.Value1), userMessage: "Value2 >= Value1");
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
        Assert.False(this.OperatorGreaterThan(l: this.Value1, r: this.NullObject), userMessage: "Value1 > NullObject");
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
    public void OperatorGreaterThanValue2IsNotGreaterThanNullObject()
    {
        Assert.False(this.OperatorGreaterThan(l: this.Value2, r: this.NullObject), userMessage: "Value2 > NullObject");
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
        Assert.True(this.OperatorLessThan(l: this.Value1, r: this.NullObject), userMessage: "NullObject < Value1");
    }

    [Fact]
    public void OperatorLessThanNullObjectIsNotLessThanValue1()
    {
        Assert.False(this.OperatorLessThan(l: this.NullObject, r: this.Value2), userMessage: "NullObject < Value2");
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
        Assert.True(this.OperatorLessThanOrEqualTo(l: this.Value1, r: this.Value2), userMessage: "Value1 <= Value2");
    }

    [Fact]
    public void OperatorLessThanOrEqualToValue1IsNotLessThanOrEquivalentToValue1()
    {
        Assert.True(this.OperatorLessThanOrEqualTo(l: this.Value1, r: this.Value1), userMessage: "Value1 <= Value1");
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
        Assert.False(this.OperatorLessThanOrEqualTo(l: this.Value2, r: this.Value1), userMessage: "Value2 <= Value1");
    }

    [Fact]
    public void OperatorLessThanValue1IsLessThanNullObject()
    {
        Assert.True(this.OperatorLessThan(l: this.Value1, r: this.NullObject), userMessage: "Value1 < NullObject");
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
    public void OperatorLessThanValue2IsLessThanNullObject()
    {
        Assert.True(this.OperatorLessThan(l: this.Value2, r: this.NullObject), userMessage: "Value2 < NullObject");
    }

    [Fact]
    public void OperatorLessThanValue2IsNotLessThanValue1()
    {
        Assert.False(this.OperatorLessThan(l: this.Value2, r: this.Value1), userMessage: "Value2 < Value1");
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
        Assert.True(TypedCompareTo(l: this.Value1, r: this.NullObject) > 0, userMessage: "Should be greater than 0");
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
        Assert.True(UntypedCompareTo(l: this.Value1, r: this.NullObject) > 0, userMessage: "Should be greater than 0");
    }

    [Fact]
    public void UntypedCompareToValue1LessThanValue2()
    {
        Assert.True(UntypedCompareTo(l: this.Value1, r: this.Value2) < 0, userMessage: "Should be less than 0");
    }

    [Fact]
    public void UntypedCompareToValue1ToOtherTypedObjectThrowsArgumentException()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(testCode: () =>
            UntypedCompareTo(l: this.Value1, Guid.NewGuid())
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
        Assert.True(UntypedCompareTo(l: this.Value2, r: this.Value1) > 0, userMessage: "Should be greater than 0");
    }

    // Single source of truth for the AOT dispatcher case table (see FunFair.Test.Source.Generator's
    // AotTestDispatcherAnalyzer, FTS002); see EquatableObjectTestBase<TObject>.BuildDispatcherCases for why this
    // must stay an ordinary static generic method rather than a [MemberData] provider itself.
    [SuppressMessage(
        category: "Meziantou.Analyzer",
        checkId: "MA0051:Method is too long",
        Justification = "Flat data table of AOT test cases, not control-flow complexity"
    )]
    [SuppressMessage(
        category: "Microsoft.Design",
        checkId: "CA1000:Do not declare static members on generic types",
        Justification = "Not a [MemberData] provider itself - a shared helper closed leaf classes call via "
            + "ComparableObjectTestBase<T>.BuildDispatcherCases<TSelf>(), avoiding a hand-copied case table per leaf"
    )]
    [SuppressMessage(
        category: "Philips.CodeAnalysis.DuplicateCodeAnalyzer",
        checkId: "PH2071:Duplicate code",
        Justification = "Structurally mirrors ComparableValueTestBase<T>.BuildDispatcherCases by design - the "
            + "object/struct dispatcher hierarchies intentionally cover the same named test cases"
    )]
    public static new (string Name, Action<TSelf> Action)[] BuildDispatcherCases<TSelf>()
        where TSelf : ComparableObjectTestBase<TObject>
    {
        return
        [
            .. EquatableObjectTestBase<TObject>.BuildDispatcherCases<TSelf>(),
            Case<TSelf>(t => t.OperatorGreaterOrEqualToThanNullObjectIsNotGreaterOrEquivalentToNullObject()),
            Case<TSelf>(t => t.OperatorGreaterThanNullObjectIsGreaterThanValue1()),
            Case<TSelf>(t => t.OperatorGreaterThanNullObjectIsNotGreaterThanNullObject()),
            Case<TSelf>(t => t.OperatorGreaterThanOrEqualToNullObjectIsGreaterThanOrEquivalentToValue1()),
            Case<TSelf>(t => t.OperatorGreaterThanOrEqualToValue1IsGreaterThanOrEquivalentToValue1()),
            Case<TSelf>(t => t.OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToNullObject()),
            Case<TSelf>(t => t.OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToValue1()),
            Case<TSelf>(t => t.OperatorGreaterThanOrEqualToValue1IsNotGreaterThanOrEquivalentToValue2()),
            Case<TSelf>(t => t.OperatorGreaterThanOrEqualToValue2IsGreaterThanOrEquivalentToValue1()),
            Case<TSelf>(t => t.OperatorGreaterThanOrEqualToValue2IsNotGreaterThanOrEquivalentToNullObject()),
            Case<TSelf>(t => t.OperatorGreaterThanValue1IsNotGreaterThanNullObject()),
            Case<TSelf>(t => t.OperatorGreaterThanValue1IsNotGreaterThanValue1()),
            Case<TSelf>(t => t.OperatorGreaterThanValue1IsNotGreaterThanValue2()),
            Case<TSelf>(t => t.OperatorGreaterThanValue2IsGreaterThanValue1()),
            Case<TSelf>(t => t.OperatorGreaterThanValue2IsNotGreaterThanNullObject()),
            Case<TSelf>(t => t.OperatorLessOrEqualToThanNullObjectIsLessThanOrEquivalentToNullObject()),
            Case<TSelf>(t => t.OperatorLessThanNullObjectIsLessThanNullObject()),
            Case<TSelf>(t => t.OperatorLessThanNullObjectIsNotLessThanValue1()),
            Case<TSelf>(t => t.OperatorLessThanOrEqualToNullObjectIsNotLessThanOrEquivalentToValue1()),
            Case<TSelf>(t => t.OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToNullObject()),
            Case<TSelf>(t => t.OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToValue1()),
            Case<TSelf>(t => t.OperatorLessThanOrEqualToValue1IsLessThanOrEquivalentToValue2()),
            Case<TSelf>(t => t.OperatorLessThanOrEqualToValue1IsNotLessThanOrEquivalentToValue1()),
            Case<TSelf>(t => t.OperatorLessThanOrEqualToValue2IsLessThanOrEquivalentToNullObject()),
            Case<TSelf>(t => t.OperatorLessThanOrEqualToValue2IsNotLessThanOrEquivalentToValue1()),
            Case<TSelf>(t => t.OperatorLessThanValue1IsLessThanNullObject()),
            Case<TSelf>(t => t.OperatorLessThanValue1IsLessThanValue2()),
            Case<TSelf>(t => t.OperatorLessThanValue1IsNotLessThanValue1()),
            Case<TSelf>(t => t.OperatorLessThanValue2IsLessThanNullObject()),
            Case<TSelf>(t => t.OperatorLessThanValue2IsNotLessThanValue1()),
            Case<TSelf>(t => t.TypedCompareToValue1EqualToEquivalentToValue1()),
            Case<TSelf>(t => t.TypedCompareToValue1GreaterThanNullObject()),
            Case<TSelf>(t => t.TypedCompareToValue1LessThanValue2()),
            Case<TSelf>(t => t.TypedCompareToValue2GreaterThanValue1()),
            Case<TSelf>(t => t.UntypedCompareToValue1EqualsUnTypedValue1Alias()),
            Case<TSelf>(t => t.UntypedCompareToValue1EqualToEquivalentToValue1()),
            Case<TSelf>(t => t.UntypedCompareToValue1GreaterThanToNullObject()),
            Case<TSelf>(t => t.UntypedCompareToValue1LessThanValue2()),
            Case<TSelf>(t => t.UntypedCompareToValue1ToOtherTypedObjectThrowsArgumentException()),
            Case<TSelf>(t => t.UntypedCompareToValue2GreaterThanUnTypedValue1Alias()),
            Case<TSelf>(t => t.UntypedCompareToValue2GreaterThanValue1()),
        ];
    }
}
