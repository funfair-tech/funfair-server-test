using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Xunit;

namespace FunFair.Test.Common;

/// <summary>
///     Base class for test objects that are equality comparable.
/// </summary>
/// <typeparam name="TObject">The object to compare.</typeparam>
public abstract class EquatableObjectTestBase<TObject> : TestBase
    where TObject : class, IEquatable<TObject>
{
    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="zeroObject">The object that's equivalent to zero or null.</param>
    /// <param name="value1">The a value to use for comparisons.</param>
    /// <param name="equivalentToValue1">An equivalent value to <paramref name="value1" /> that is not ReferenceEqual to <paramref name="value1" />.</param>
    protected EquatableObjectTestBase(TObject zeroObject, TObject value1, TObject equivalentToValue1)
    {
        this.ZeroObject = zeroObject;
        this.Value1 = value1;
        this.Value1Alias = value1;
        this.EquivalentToValue1 = equivalentToValue1;
        this.EquivalentToValue1AsObject = equivalentToValue1;
        this.NullObject = null;
    }

    /// <summary>
    ///     An object that is equivalent or as near as possible to zero
    /// </summary>
    [SuppressMessage(category: "ReSharper", checkId: "MemberCanBePrivate.Global", Justification = "TODO: Add Unit tests")]
    protected internal TObject ZeroObject { get; }

    /// <summary>
    ///     The Value.
    /// </summary>
    [SuppressMessage(category: "ReSharper", checkId: "MemberCanBePrivate.Global", Justification = "TODO: Add Unit tests")]
    protected internal TObject Value1 { get; }

    /// <summary>
    ///     An alias of the value.
    /// </summary>
    [SuppressMessage(category: "ReSharper", checkId: "MemberCanBePrivate.Global", Justification = "TODO: Add Unit tests")]
    protected internal TObject Value1Alias { get; }

    /// <summary>
    ///     A value that is equivalent to value 1
    /// </summary>
    [SuppressMessage(category: "ReSharper", checkId: "MemberCanBePrivate.Global", Justification = "TODO: Add Unit tests")]
    protected internal TObject EquivalentToValue1 { get; }

    /// <summary>
    ///     A value that is equivalent to value 1, but typed as an object.
    /// </summary>
    [SuppressMessage(category: "ReSharper", checkId: "MemberCanBePrivate.Global", Justification = "TODO: Add Unit tests")]
    protected internal object EquivalentToValue1AsObject { get; }

    /// <summary>
    ///     A Null object.
    /// </summary>
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

    /// <summary>
    ///     Implementation should call operator ==(x,y)
    /// </summary>
    /// <param name="x">An item</param>
    /// <param name="y">Another item.</param>
    /// <returns>True if the items are the same; otherwise, false.</returns>
    protected abstract bool OperatorEquals(TObject? x, TObject? y);

    /// <summary>
    ///     Implementation should call operator !=(x,y)
    /// </summary>
    /// <param name="x">An item</param>
    /// <param name="y">Another item.</param>
    /// <returns>True if the items are different; otherwise, false.</returns>
    protected abstract bool OperatorNotEquals(TObject? x, TObject? y);

    /// <summary>
    ///     Checks that Get Hash Code is stable
    /// </summary>
    [Fact]
    public void GetHashCodeSameNoMatterHowManyTimesCalled()
    {
        int referenceHashCode = this.Value1.GetHashCode();

        int[] selection = Enumerable.Range(start: 0, count: 100)
                                    .Select(selector: _ => this.Value1.GetHashCode())
                                    .ToArray();

        Assert.All(collection: selection, action: hashCode => Assert.Equal(expected: hashCode, actual: referenceHashCode));
    }

    /// <summary>
    ///     Compares the hash codes of Value1 and EquivalentToValue1.
    /// </summary>
    [Fact]
    public void GetHashCodeValue1ObjectIsSameAsEquivalentToValue1Object()
    {
        Assert.False(ReferenceEquals(objA: this.Value1, objB: this.EquivalentToValue1), userMessage: "Should not be same object instance");
        Assert.Equal(this.Value1.GetHashCode(), this.EquivalentToValue1.GetHashCode());
    }

    /// <summary>
    ///     Compares the hash codes of Value1 and Value1Alias.
    /// </summary>
    [Fact]
    public void GetHashCodeValue1ObjectIsSameAsValue1AliasObject()
    {
        Assert.Equal(this.Value1.GetHashCode(), this.Value1Alias.GetHashCode());
    }

    /// <summary>
    ///     Compares the hash codes of Value1 and Value1.
    /// </summary>
    [Fact]
    public void GetHashCodeValue1ObjectIsSameAsValue1Object()
    {
        Assert.Equal(this.Value1.GetHashCode(), this.Value1.GetHashCode());
    }

    /// <summary>
    ///     Compares the hash codes of ZeroObject and ZeroObject.
    /// </summary>
    [Fact]
    public void GetHashCodeZeroObjectIsSameAsZeroObject()
    {
        Assert.Equal(this.ZeroObject.GetHashCode(), this.ZeroObject.GetHashCode());
    }

    /// <summary>
    ///     Compares NullObject and ZeroObject.
    /// </summary>
    [Fact]
    public void OperatorEqualsNullObjectDifferentToZeroObject()
    {
        Assert.False(this.OperatorEquals(x: this.NullObject, y: this.ZeroObject), userMessage: "Should Be different");
    }

    /// <summary>
    ///     Compares NullObject and NullObject.
    /// </summary>
    [Fact]
    public void OperatorEqualsNullObjectSameAsNullObject()
    {
        Assert.True(this.OperatorEquals(x: this.NullObject, y: this.NullObject), userMessage: "Should Be different");
    }

    /// <summary>
    ///     Compares Value1 and EquivalentToValue1.
    /// </summary>
    [Fact]
    public void OperatorEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
    {
        Assert.False(ReferenceEquals(objA: this.Value1, objB: this.EquivalentToValue1), userMessage: "Should not be same object instance");
        Assert.True(this.OperatorEquals(x: this.Value1, y: this.EquivalentToValue1), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares Value1 and Value1Alias.
    /// </summary>
    [Fact]
    public void OperatorEqualsValue1ObjectIsSameAsValue1AliasObject()
    {
        Assert.True(this.OperatorEquals(x: this.Value1, y: this.Value1Alias), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares Value1 and Value1.
    /// </summary>
    [Fact]
    public void OperatorEqualsValue1ObjectIsSameAsValue1Object()
    {
        Assert.True(this.OperatorEquals(x: this.Value1, y: this.Value1), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares ZeroObject and NullObject.
    /// </summary>
    [Fact]
    public void OperatorEqualsZeroObjectDifferentToNullObject()
    {
        Assert.False(this.OperatorEquals(x: this.ZeroObject, y: this.NullObject), userMessage: "Should Be different");
    }

    /// <summary>
    ///     Compares ZeroObject and ZeroObject.
    /// </summary>
    [Fact]
    public void OperatorEqualsZeroObjectIsSameAsZeroObject()
    {
        Assert.True(this.OperatorEquals(x: this.ZeroObject, y: this.ZeroObject), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares NullObject and ZeroObject.
    /// </summary>
    [Fact]
    public void OperatorNotEqualsNullObjectDifferentToZeroObject()
    {
        Assert.True(this.OperatorNotEquals(x: this.NullObject, y: this.ZeroObject), userMessage: "Should Be different");
    }

    /// <summary>
    ///     Compares NullObject and NullObject.
    /// </summary>
    [Fact]
    public void OperatorNotEqualsNullObjectSameAsNullObject()
    {
        Assert.False(this.OperatorNotEquals(x: this.NullObject, y: this.NullObject), userMessage: "Should Be different");
    }

    /// <summary>
    ///     Compares Value1 and EquivalentToValue1.
    /// </summary>
    [Fact]
    public void OperatorNotEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
    {
        Assert.False(ReferenceEquals(objA: this.Value1, objB: this.EquivalentToValue1), userMessage: "Should not be same object instance");
        Assert.False(this.OperatorNotEquals(x: this.Value1, y: this.EquivalentToValue1), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares Value1 and Value1Alias.
    /// </summary>
    [Fact]
    public void OperatorNotEqualsValue1ObjectIsSameAsValue1AliasObject()
    {
        Assert.False(this.OperatorNotEquals(x: this.Value1, y: this.Value1Alias), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares Value1 and Value1.
    /// </summary>
    [Fact]
    public void OperatorNotEqualsValue1ObjectIsSameAsValue1Object()
    {
        Assert.False(this.OperatorNotEquals(x: this.Value1, y: this.Value1), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares ZeroObject and NullObject.
    /// </summary>
    [Fact]
    public void OperatorNotEqualsZeroObjectDifferentToNullObject()
    {
        Assert.True(this.OperatorNotEquals(x: this.ZeroObject, y: this.NullObject), userMessage: "Should Be different");
    }

    /// <summary>
    ///     Compares ZeroObject and ZeroObject.
    /// </summary>
    [Fact]
    public void OperatorNotEqualsZeroObjectIsSameAsZeroObject()
    {
        Assert.False(this.OperatorNotEquals(x: this.ZeroObject, y: this.ZeroObject), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares Value1 and EquivalentToValue1.
    /// </summary>
    [Fact]
    public void TypedEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
    {
        Assert.False(ReferenceEquals(objA: this.Value1, objB: this.EquivalentToValue1), userMessage: "Should not be same object instance");
        Assert.True(TypedEquals(x: this.Value1, y: this.EquivalentToValue1), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares Value1 and Value1Alias.
    /// </summary>
    [Fact]
    public void TypedEqualsValue1ObjectIsSameAsValue1AliasObject()
    {
        Assert.True(TypedEquals(x: this.Value1, y: this.Value1Alias), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares Value1 and Value1A.
    /// </summary>
    [Fact]
    public void TypedEqualsValue1ObjectIsSameAsValue1Object()
    {
        Assert.True(TypedEquals(x: this.Value1, y: this.Value1), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares ZeroObject and NullObject.
    /// </summary>
    [Fact]
    public void TypedEqualsZeroObjectDifferentToNullObject()
    {
        Assert.False(TypedEquals(x: this.ZeroObject, y: this.NullObject), userMessage: "Should Be different");
    }

    /// <summary>
    ///     Compares ZeroObject and ZeroObject.
    /// </summary>
    [Fact]
    public void TypedEqualsZeroObjectIsSameAsZeroObject()
    {
        Assert.True(TypedEquals(x: this.ZeroObject, y: this.ZeroObject), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares Value1 and EquivalentToValue1.
    /// </summary>
    [Fact]
    public void UntypedEqualsValue1ObjectIsSameAsEquivalentToValue1Object()
    {
        Assert.False(ReferenceEquals(objA: this.Value1, objB: this.EquivalentToValue1), userMessage: "Should not be same object instance");
        Assert.True(UntypedEquals(x: this.Value1, y: this.EquivalentToValue1), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares Value1 and EquivalentToValue1AsObject.
    /// </summary>
    [Fact]
    public void UntypedEqualsValue1ObjectIsSameAsEquivalentToValue1ObjectAsObject()
    {
        Assert.False(ReferenceEquals(objA: this.Value1, objB: this.EquivalentToValue1AsObject), userMessage: "Should not be same object instance");
        Assert.True(UntypedEquals(x: this.Value1, y: this.EquivalentToValue1AsObject), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares Value1 and Value1Alias.
    /// </summary>
    [Fact]
    public void UntypedEqualsValue1ObjectIsSameAsValue1AliasObject()
    {
        Assert.True(UntypedEquals(x: this.Value1, y: this.Value1Alias), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares Value1 and Value1.
    /// </summary>
    [Fact]
    public void UntypedEqualsValue1ObjectIsSameAsValue1Object()
    {
        Assert.True(UntypedEquals(x: this.Value1, y: this.Value1), userMessage: "Should Be Same");
    }

    /// <summary>
    ///     Compares ZeroObject and Banana.
    /// </summary>
    [Fact]
    public void UntypedEqualsZeroObjectDifferentToAnotherTypeOfObject()
    {
        Assert.False(UntypedEquals(x: this.ZeroObject, Guid.NewGuid()), userMessage: "Should Be different");
    }

    /// <summary>
    ///     Compares ZeroObject and NullObject.
    /// </summary>
    [Fact]
    public void UntypedEqualsZeroObjectDifferentToNullObject()
    {
        Assert.False(UntypedEquals(x: this.ZeroObject, y: this.NullObject), userMessage: "Should Be different");
    }

    /// <summary>
    ///     Compares ZeroObject and ZeroObject.
    /// </summary>
    [Fact]
    public void UntypedEqualsZeroObjectIsSameAsZeroObject()
    {
        Assert.True(UntypedEquals(x: this.ZeroObject, y: this.ZeroObject), userMessage: "Should Be Same");
    }
}