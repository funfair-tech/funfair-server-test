using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace FunFair.Test.Common;

public abstract class EquatableObjectTestBase<TObject> : TestBase
    where TObject : class, IEquatable<TObject>
{
    protected EquatableObjectTestBase(TObject zeroObject, TObject value1, TObject equivalentToValue1)
    {
        this.ZeroObject = zeroObject;
        this.Value1 = value1;
        this.Value1Alias = value1;
        this.EquivalentToValue1 = equivalentToValue1;
        this.EquivalentToValue1AsObject = equivalentToValue1;
        this.NullObject = null;
    }

    protected internal TObject ZeroObject { get; }

    protected internal TObject Value1 { get; }

    protected internal TObject Value1Alias { get; }

    protected internal TObject EquivalentToValue1 { get; }

    protected internal object EquivalentToValue1AsObject { get; }

    protected internal TObject? NullObject { get; }

    private static bool TypedEquals(TObject x, TObject? y)
    {
        IEquatable<TObject> eq = x;

        return eq.Equals(y);
    }

    private static bool UntypedEquals(TObject x, object? y)
    {
        return x.Equals(y);
    }

    protected abstract bool OperatorEquals(TObject? x, TObject? y);

    protected abstract bool OperatorNotEquals(TObject? x, TObject? y);

    [Fact]
    public void GetHashCodeSameNoMatterHowManyTimesCalled()
    {
        int referenceHashCode = this.Value1.GetHashCode();

        IReadOnlyList<int> selection = this.GetHashCodes();

        Assert.All(collection: selection, action: hashCode => Assert.Equal(expected: hashCode, actual: referenceHashCode));
    }

    private IReadOnlyList<int> GetHashCodes()
    {
        return
        [
            ..Enumerable.Range(start: 0, count: 100)
                        .Select(selector: _ => this.Value1.GetHashCode())
        ];
    }

    [Fact]
    public void GetHashCodeValue1ObjectIsSameAsEquivalentToValue1Object()
    {
        Assert.False(ReferenceEquals(objA: this.Value1, objB: this.EquivalentToValue1), userMessage: "Should not be same object instance");
        Assert.Equal(this.Value1.GetHashCode(), this.EquivalentToValue1.GetHashCode());
    }

    [Fact]
    public void GetHashCodeValue1ObjectIsSameAsValue1AliasObject()
    {
        Assert.Equal(this.Value1.GetHashCode(), this.Value1Alias.GetHashCode());
    }

    [Fact]
    public void GetHashCodeValue1ObjectIsSameAsValue1Object()
    {
        Assert.Equal(this.Value1.GetHashCode(), this.Value1.GetHashCode());
    }

    [Fact]
    public void GetHashCodeZeroObjectIsSameAsZeroObject()
    {
        Assert.Equal(this.ZeroObject.GetHashCode(), this.ZeroObject.GetHashCode());
    }

    [Fact]
    public void OperatorEqualsNullObjectDifferentToZeroObject()
    {
        Assert.False(this.OperatorEquals(x: this.NullObject, y: this.ZeroObject), userMessage: "Should Be different");
    }

    [Fact]
    public void OperatorEqualsNullObjectSameAsNullObject()
    {
        Assert.True(this.OperatorEquals(x: this.NullObject, y: this.NullObject), userMessage: "Should Be different");
    }

    [Fact]
    public void OperatorEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
    {
        Assert.False(ReferenceEquals(objA: this.Value1, objB: this.EquivalentToValue1), userMessage: "Should not be same object instance");
        Assert.True(this.OperatorEquals(x: this.Value1, y: this.EquivalentToValue1), userMessage: "Should Be Same");
    }

    [Fact]
    public void OperatorEqualsValue1ObjectIsSameAsValue1AliasObject()
    {
        Assert.True(this.OperatorEquals(x: this.Value1, y: this.Value1Alias), userMessage: "Should Be Same");
    }

    [Fact]
    public void OperatorEqualsValue1ObjectIsSameAsValue1Object()
    {
        Assert.True(this.OperatorEquals(x: this.Value1, y: this.Value1), userMessage: "Should Be Same");
    }

    [Fact]
    public void OperatorEqualsZeroObjectDifferentToNullObject()
    {
        Assert.False(this.OperatorEquals(x: this.ZeroObject, y: this.NullObject), userMessage: "Should Be different");
    }

    [Fact]
    public void OperatorEqualsZeroObjectIsSameAsZeroObject()
    {
        Assert.True(this.OperatorEquals(x: this.ZeroObject, y: this.ZeroObject), userMessage: "Should Be Same");
    }

    [Fact]
    public void OperatorNotEqualsNullObjectDifferentToZeroObject()
    {
        Assert.True(this.OperatorNotEquals(x: this.NullObject, y: this.ZeroObject), userMessage: "Should Be different");
    }

    [Fact]
    public void OperatorNotEqualsNullObjectSameAsNullObject()
    {
        Assert.False(this.OperatorNotEquals(x: this.NullObject, y: this.NullObject), userMessage: "Should Be different");
    }

    [Fact]
    public void OperatorNotEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
    {
        Assert.False(ReferenceEquals(objA: this.Value1, objB: this.EquivalentToValue1), userMessage: "Should not be same object instance");
        Assert.False(this.OperatorNotEquals(x: this.Value1, y: this.EquivalentToValue1), userMessage: "Should Be Same");
    }

    [Fact]
    public void OperatorNotEqualsValue1ObjectIsSameAsValue1AliasObject()
    {
        Assert.False(this.OperatorNotEquals(x: this.Value1, y: this.Value1Alias), userMessage: "Should Be Same");
    }

    [Fact]
    public void OperatorNotEqualsValue1ObjectIsSameAsValue1Object()
    {
        Assert.False(this.OperatorNotEquals(x: this.Value1, y: this.Value1), userMessage: "Should Be Same");
    }

    [Fact]
    public void OperatorNotEqualsZeroObjectDifferentToNullObject()
    {
        Assert.True(this.OperatorNotEquals(x: this.ZeroObject, y: this.NullObject), userMessage: "Should Be different");
    }

    [Fact]
    public void OperatorNotEqualsZeroObjectIsSameAsZeroObject()
    {
        Assert.False(this.OperatorNotEquals(x: this.ZeroObject, y: this.ZeroObject), userMessage: "Should Be Same");
    }

    [Fact]
    public void TypedEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
    {
        Assert.False(ReferenceEquals(objA: this.Value1, objB: this.EquivalentToValue1), userMessage: "Should not be same object instance");
        Assert.True(TypedEquals(x: this.Value1, y: this.EquivalentToValue1), userMessage: "Should Be Same");
    }

    [Fact]
    public void TypedEqualsValue1ObjectIsSameAsValue1AliasObject()
    {
        Assert.True(TypedEquals(x: this.Value1, y: this.Value1Alias), userMessage: "Should Be Same");
    }

    [Fact]
    public void TypedEqualsValue1ObjectIsSameAsValue1Object()
    {
        Assert.True(TypedEquals(x: this.Value1, y: this.Value1), userMessage: "Should Be Same");
    }

    [Fact]
    public void TypedEqualsZeroObjectDifferentToNullObject()
    {
        Assert.False(TypedEquals(x: this.ZeroObject, y: this.NullObject), userMessage: "Should Be different");
    }

    [Fact]
    public void TypedEqualsZeroObjectIsSameAsZeroObject()
    {
        Assert.True(TypedEquals(x: this.ZeroObject, y: this.ZeroObject), userMessage: "Should Be Same");
    }

    [Fact]
    public void UntypedEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
    {
        Assert.False(ReferenceEquals(objA: this.Value1, objB: this.EquivalentToValue1), userMessage: "Should not be same object instance");
        Assert.True(UntypedEquals(x: this.Value1, y: this.EquivalentToValue1), userMessage: "Should Be Same");
    }

    [Fact]
    public void UntypedEqualsValue1ObjectIsSameAsEquivalentToValue1ObjectAsObject()
    {
        Assert.False(ReferenceEquals(objA: this.Value1, objB: this.EquivalentToValue1AsObject), userMessage: "Should not be same object instance");
        Assert.True(UntypedEquals(x: this.Value1, y: this.EquivalentToValue1AsObject), userMessage: "Should Be Same");
    }

    [Fact]
    public void UntypedEqualsValue1ObjectIsSameAsValue1AliasObject()
    {
        Assert.True(UntypedEquals(x: this.Value1, y: this.Value1Alias), userMessage: "Should Be Same");
    }

    [Fact]
    public void UntypedEqualsValue1ObjectIsSameAsValue1Object()
    {
        Assert.True(UntypedEquals(x: this.Value1, y: this.Value1), userMessage: "Should Be Same");
    }

    [Fact]
    public void UntypedEqualsZeroObjectDifferentToAnotherTypeOfObject()
    {
        Assert.False(UntypedEquals(x: this.ZeroObject, Guid.NewGuid()), userMessage: "Should Be different");
    }

    [Fact]
    public void UntypedEqualsZeroObjectDifferentToNullObject()
    {
        Assert.False(UntypedEquals(x: this.ZeroObject, y: this.NullObject), userMessage: "Should Be different");
    }

    [Fact]
    public void UntypedEqualsZeroObjectIsSameAsZeroObject()
    {
        Assert.True(UntypedEquals(x: this.ZeroObject, y: this.ZeroObject), userMessage: "Should Be Same");
    }
}