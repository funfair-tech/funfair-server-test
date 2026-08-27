using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Xunit;
using static FunFair.Test.Common.DispatcherCaseData;

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

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool TypedEquals(in TObject x, in TObject y)
    {
        return DoTypedEquals(l: x, r: y);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool DoTypedEquals<T>(in T l, in T r)
        where T : struct, IEquatable<T>
    {
        return l.Equals(r);
    }

    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        IReadOnlyList<int> selection = this.GetHashCodes();

        Assert.All(
            collection: selection,
            action: hashCode => Assert.Equal(expected: hashCode, actual: referenceHashCode)
        );
    }

    private int[] GetHashCodes()
    {
        int[] hashCodes = new int[100];

        for (int i = 0; i < hashCodes.Length; i++)
        {
            hashCodes[i] = this.Value1.GetHashCode();
        }

        return hashCodes;
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
            + "EquatableValueTestBase<T>.BuildDispatcherCases<TSelf>(), avoiding a hand-copied case table per leaf"
    )]
    [SuppressMessage(
        category: "Philips.CodeAnalysis.DuplicateCodeAnalyzer",
        checkId: "PH2071:Duplicate code",
        Justification = "Structurally mirrors EquatableObjectTestBase<T>.BuildDispatcherCases by design - the "
            + "object/struct dispatcher hierarchies intentionally cover the same named test cases"
    )]
    public static (string Name, Action<TSelf> Action)[] BuildDispatcherCases<TSelf>()
        where TSelf : EquatableValueTestBase<TObject>
    {
        return
        [
            Case<TSelf>(t => t.GetHashCodeSameNoMatterHowManyTimesCalled()),
            Case<TSelf>(t => t.GetHashCodeValue1ObjectIsSameAsEquivalentToValue1Object()),
            Case<TSelf>(t => t.GetHashCodeValue1ObjectIsSameAsValue1AliasObject()),
            Case<TSelf>(t => t.GetHashCodeValue1ObjectIsSameAsValue1Object()),
            Case<TSelf>(t => t.GetHashCodeZeroObjectIsSameAsZeroObject()),
            Case<TSelf>(t => t.OperatorEqualsValue1ObjectIsSameAsEquivalentToValue1Object()),
            Case<TSelf>(t => t.OperatorEqualsValue1ObjectIsSameAsValue1AliasObject()),
            Case<TSelf>(t => t.OperatorEqualsValue1ObjectIsSameAsValue1Object()),
            Case<TSelf>(t => t.OperatorEqualsZeroObjectIsSameAsZeroObject()),
            Case<TSelf>(t => t.OperatorNotEqualsValue1ObjectIsSameAsEquivalentToValue1Object()),
            Case<TSelf>(t => t.OperatorNotEqualsValue1ObjectIsSameAsValue1AliasObject()),
            Case<TSelf>(t => t.OperatorNotEqualsValue1ObjectIsSameAsValue1Object()),
            Case<TSelf>(t => t.OperatorNotEqualsZeroObjectIsSameAsZeroObject()),
            Case<TSelf>(t => t.TypedEqualsValue1ObjectIsSameAsEquivalentToValue1Object()),
            Case<TSelf>(t => t.TypedEqualsValue1ObjectIsSameAsValue1AliasObject()),
            Case<TSelf>(t => t.TypedEqualsValue1ObjectIsSameAsValue1Object()),
            Case<TSelf>(t => t.TypedEqualsZeroObjectIsSameAsZeroObject()),
            Case<TSelf>(t => t.UntypedEqualsValue1ObjectIsSameAsEquivalentToValue1Object()),
            Case<TSelf>(t => t.UntypedEqualsValue1ObjectIsSameAsEquivalentToValue1ObjectAsObject()),
            Case<TSelf>(t => t.UntypedEqualsValue1ObjectIsSameAsValue1AliasObject()),
            Case<TSelf>(t => t.UntypedEqualsValue1ObjectIsSameAsValue1Object()),
            Case<TSelf>(t => t.UntypedEqualsZeroObjectDifferentToAnotherTypeOfObject()),
            Case<TSelf>(t => t.UntypedEqualsZeroObjectIsSameAsZeroObject()),
        ];
    }
}
