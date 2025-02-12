using System;
using FunFair.Test.Common.Helpers;
using FunFair.Test.Common.Tests.Mocks;
using Xunit;

namespace FunFair.Test.Common.Tests.Helpers;

public sealed class ReferenceObjectHelperTests : TestBase
{
    private readonly Func<MockGenericModel<int>, MockGenericModel<int>, int> _compare = (left, right) => left.Value.CompareTo(right.Value);

    private readonly Func<MockGenericModel<int>, MockGenericModel<int>, bool> _equals = (left, right) => left.Equals(right);

    [Fact]
    public void ObjectsAreEqualIfTheirNonReferencePartsAreEquals()
    {
        const int value = 1;
        MockGenericModel<int> left = new(value);
        MockGenericModel<int> right = new(value);

        Assert.True(ReferenceObjectHelpers.AreEqual(left: left, right: right, eq: (l, r) => l.Value.Equals(r.Value)), userMessage: "Should be same");
    }

    [Fact]
    public void ObjectsAreEqualIfTheyAreSameReference()
    {
        MockGenericModel<int> obj = new(value: 1);

        Assert.True(ReferenceObjectHelpers.AreEqual(left: obj, right: obj, eq: this._equals), userMessage: "Should be same");
    }

    [Fact]
    public void ObjectsAreNotEqualIfLeftIsNull()
    {
        MockGenericModel<int>? left = null;
        MockGenericModel<int> right = new(value: 1);

        Assert.False(ReferenceObjectHelpers.AreEqual(left: left, right: right, eq: this._equals), userMessage: "Should be different");
    }

    [Fact]
    public void ObjectsAreNotEqualIfRightIsNull()
    {
        MockGenericModel<int> left = new(value: 1);
        MockGenericModel<int>? right = null;

        Assert.False(ReferenceObjectHelpers.AreEqual(left: left, right: right, eq: this._equals), userMessage: "Should be different");
    }

    [Fact]
    public void ObjectsAreNotEqualIfTheirNonReferencePartsAreNotEquals()
    {
        MockGenericModel<int> left = new(value: 1);
        MockGenericModel<int> right = new(value: 2);

        Assert.False(ReferenceObjectHelpers.AreEqual(left: left, right: right, eq: this._equals), userMessage: "Should be different");
    }

    [Fact]
    public void ObjectsAreNotSameIfLeftIsNull()
    {
        MockGenericModel<int>? left = null;
        MockGenericModel<int> right = new(value: 1);

        Assert.Equal(expected: 1, ReferenceObjectHelpers.Compare(left: left, right: right, cmp: this._compare));
    }

    [Fact]
    public void ObjectsAreNotSameIfLeftNonReferenceIsBigger()
    {
        MockGenericModel<int> left = new(value: 2);
        MockGenericModel<int> right = new(value: 1);

        Assert.Equal(expected: 1, ReferenceObjectHelpers.Compare(left: left, right: right, cmp: this._compare));
    }

    [Fact]
    public void ObjectsAreNotSameIfLeftNonReferenceIsLess()
    {
        MockGenericModel<int> left = new(value: 1);
        MockGenericModel<int> right = new(value: 2);

        Assert.Equal(expected: -1, ReferenceObjectHelpers.Compare(left: left, right: right, cmp: this._compare));
    }

    [Fact]
    public void ObjectsAreNotSameIfRightIsNull()
    {
        MockGenericModel<int> left = new(value: 1);
        MockGenericModel<int>? right = null;

        Assert.Equal(expected: -1, ReferenceObjectHelpers.Compare(left: left, right: right, cmp: this._compare));
    }

    [Fact]
    public void ObjectsAreSameIfTheirNonReferencePartsAreEquals()
    {
        const int value = 1;
        MockGenericModel<int> left = new(value);
        MockGenericModel<int> right = new(value);

        Assert.Equal(expected: 0, ReferenceObjectHelpers.Compare(left: left, right: right, cmp: this._compare));
    }

    [Fact]
    public void ObjectsAreSameIfTheyAreSameReference()
    {
        MockGenericModel<int> obj = new(value: 1);

        Assert.Equal(expected: 0, ReferenceObjectHelpers.Compare(left: obj, right: obj, cmp: this._compare));
    }
}
