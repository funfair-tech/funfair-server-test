using System;
using System.Linq;
using Xunit;

namespace FunFair.Test.Common;

public abstract class EquatableValueTestBase<TObject> : TestBase
    where TObject : struct, IEquatable<TObject>
{
    protected EquatableValueTestBase(TObject zeroObject, TObject value1, TObject equivalentToValue1)
    {
        this.ZeroObject = zeroObject;
        this.Value1 = value1;
        this.Value1Alias = value1;
        this.EquivalentToValue1 = equivalentToValue1;
        this.EquivalentToValue1AsObject = equivalentToValue1;
    }

    protected internal TObject ZeroObject { get; }

    protected internal TObject Value1 { get; }

    protected internal TObject Value1Alias { get; }

    protected internal TObject EquivalentToValue1 { get; }

    protected internal object EquivalentToValue1AsObject { get; }

    private static bool TypedEquals(in TObject x, in TObject y)
    {
        IEquatable<TObject> eq = x;

        return eq.Equals(y);
    }

    private static bool UntypedEquals(in TObject x, object? y)
    {
        return x.Equals(y);
    }

    protected abstract bool OperatorEquals(in TObject x, in TObject y);

    protected abstract bool OperatorNotEquals(in TObject x, in TObject y);

    [Fact]
    public void GetHashCodeSameNoMatterHowManyTimesCalled()
    {
        int referenceHashCode = this.Value1.GetHashCode();

        int[] selection = this.GetHashCodes();

        Assert.All(collection: selection, action: hashCode => Assert.Equal(expected: hashCode, actual: referenceHashCode));
    }

    private int[] GetHashCodes()
    {
        return Enumerable.Range(start: 0, count: 100)
                         .Select(selector: _ => this.Value1.GetHashCode())
                         .ToArray();
    }

    [Fact]
    public void GetHashCodeValue1ObjectIsSameAsEquivalentToValue1Object()
    {
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
    public void OperatorEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
    {
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
    public void OperatorEqualsZeroObjectIsSameAsZeroObject()
    {
        Assert.True(this.OperatorEquals(x: this.ZeroObject, y: this.ZeroObject), userMessage: "Should Be Same");
    }

    [Fact]
    public void OperatorNotEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
    {
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
    public void OperatorNotEqualsZeroObjectIsSameAsZeroObject()
    {
        Assert.False(this.OperatorNotEquals(x: this.ZeroObject, y: this.ZeroObject), userMessage: "Should Be Same");
    }

    [Fact]
    public void TypedEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
    {
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
    public void TypedEqualsZeroObjectIsSameAsZeroObject()
    {
        Assert.True(TypedEquals(x: this.ZeroObject, y: this.ZeroObject), userMessage: "Should Be Same");
    }

    [Fact]
    public void UntypedEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
    {
        Assert.True(UntypedEquals(x: this.Value1, y: this.EquivalentToValue1), userMessage: "Should Be Same");
    }

    [Fact]
    public void UntypedEqualsValue1ObjectIsSameAsEquivalentToValue1ObjectAsObject()
    {
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
    public void UntypedEqualsZeroObjectIsSameAsZeroObject()
    {
        Assert.True(UntypedEquals(x: this.ZeroObject, y: this.ZeroObject), userMessage: "Should Be Same");
    }
}