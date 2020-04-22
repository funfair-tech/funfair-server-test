using System;
using FunFair.Test.Common.Helpers;
using FunFair.Test.Common.Tests.Mocks;
using Xunit;

namespace FunFair.Test.Common.Tests.Helpers
{
    public sealed class ReferenceObjectHelperTests : TestBase
    {
        private readonly Func<MockGenericModel<int>, MockGenericModel<int>, int> _compare = (left, right) => left.Value.CompareTo(right.Value);

        private readonly Func<MockGenericModel<int>, MockGenericModel<int>, bool> _equals = (left, right) => left.Equals(right);

        [Fact]
        public void ObjectsAreEqualIfTheirNonReferencePartsAreEquals()
        {
            const int value = 1;
            MockGenericModel<int> left = new MockGenericModel<int>(value);
            MockGenericModel<int> right = new MockGenericModel<int>(value);

            Assert.True(ReferenceObjectHelpers.AreEqual(left, right, eq: (l, r) => l.Value.Equals(r.Value)), userMessage: "Should be same");
        }

        [Fact]
        public void ObjectsAreEqualIfTheyAreSameReference()
        {
            MockGenericModel<int> obj = new MockGenericModel<int>(value: 1);

            Assert.True(ReferenceObjectHelpers.AreEqual(obj, obj, this._equals), userMessage: "Should be same");
        }

        [Fact]
        public void ObjectsAreNotEqualIfLeftIsNull()
        {
            MockGenericModel<int> right = new MockGenericModel<int>(value: 1);

            Assert.False(ReferenceObjectHelpers.AreEqual(left: null, right, this._equals), userMessage: "Should be different");
        }

        [Fact]
        public void ObjectsAreNotEqualIfRightIsNull()
        {
            MockGenericModel<int> left = new MockGenericModel<int>(value: 1);

            Assert.False(ReferenceObjectHelpers.AreEqual(left, right: null, this._equals), userMessage: "Should be different");
        }

        [Fact]
        public void ObjectsAreNotEqualIfTheirNonReferencePartsAreNotEquals()
        {
            MockGenericModel<int> left = new MockGenericModel<int>(value: 1);
            MockGenericModel<int> right = new MockGenericModel<int>(value: 2);

            Assert.False(ReferenceObjectHelpers.AreEqual(left, right, this._equals), userMessage: "Should be different");
        }

        [Fact]
        public void ObjectsAreNotSameIfLeftIsNull()
        {
            MockGenericModel<int> right = new MockGenericModel<int>(value: 1);

            Assert.Equal(expected: 1, ReferenceObjectHelpers.Compare(left: null, right, this._compare));
        }

        [Fact]
        public void ObjectsAreNotSameIfLeftNonReferenceIsBigger()
        {
            MockGenericModel<int> left = new MockGenericModel<int>(value: 2);
            MockGenericModel<int> right = new MockGenericModel<int>(value: 1);

            Assert.Equal(expected: 1, ReferenceObjectHelpers.Compare(left, right, this._compare));
        }

        [Fact]
        public void ObjectsAreNotSameIfLeftNonReferenceIsLess()
        {
            MockGenericModel<int> left = new MockGenericModel<int>(value: 1);
            MockGenericModel<int> right = new MockGenericModel<int>(value: 2);

            Assert.Equal(expected: -1, ReferenceObjectHelpers.Compare(left, right, this._compare));
        }

        [Fact]
        public void ObjectsAreNotSameIfRightIsNull()
        {
            MockGenericModel<int> left = new MockGenericModel<int>(value: 1);

            Assert.Equal(expected: -1, ReferenceObjectHelpers.Compare(left, right: null, this._compare));
        }

        [Fact]
        public void ObjectsAreSameIfTheirNonReferencePartsAreEquals()
        {
            const int value = 1;
            MockGenericModel<int> left = new MockGenericModel<int>(value);
            MockGenericModel<int> right = new MockGenericModel<int>(value);

            Assert.Equal(expected: 0, ReferenceObjectHelpers.Compare(left, right, this._compare));
        }

        [Fact]
        public void ObjectsAreSameIfTheyAreSameReference()
        {
            MockGenericModel<int> obj = new MockGenericModel<int>(value: 1);

            Assert.Equal(expected: 0, ReferenceObjectHelpers.Compare(obj, obj, this._compare));
        }
    }
}