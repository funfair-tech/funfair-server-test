using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FunFair.Test.Common.Mocks;
using Xunit;
using Xunit.Sdk;

namespace FunFair.Test.Common.Tests.Mocks
{
    public sealed class ExtendedAssertTest
    {
        private static MockGenericModel<string> CreateModel(string value)
        {
            return new MockGenericModel<string>(value);
        }

        private static IReadOnlyList<MockGenericModel<string>> CreateModelList(string value)
        {
            return new ReadOnlyCollection<MockGenericModel<string>>(new List<MockGenericModel<string>>() { CreateModel(value) });
        }

        private readonly Func<MockGenericModel<int>, MockGenericModel<int>, int> _compare = (left, right) => left.Value.CompareTo(right.Value);

        private readonly Func<MockGenericModel<int>, MockGenericModel<int>, bool> _equals = (left, right) => left.Equals(right);

        [Fact]
        public void TwoObjectsAreDeepEqualIfAllValuesAreEquals()
        {
            MockGenericModel<string> expected = CreateModel(value: "expected");
            MockGenericModel<string> actual = CreateModel(value: "expected");

            ExtendedAssert.DeepEqual(expected, actual);
        }

        [Fact]
        public void TwoObjectsAreNotDeepEqualIfAllValuesAreEquals()
        {
            MockGenericModel<string> expected = CreateModel(value: "expected");
            MockGenericModel<string> actual = CreateModel(value: "actual");

            Assert.Throws<EqualException>(testCode: () => { ExtendedAssert.DeepEqual(expected, actual); });
        }

        [Fact]
        public void TwoObjectsAreNotDeepEqualIfAnyNestedValueIsNotEqual()
        {
            MockGenericModel<string> expected = CreateModel(value: "expected");
            expected.NestedValue = new[] { "new nested value" };
            MockGenericModel<string> actual = CreateModel(value: "expected");

            Assert.Throws<EqualException>(testCode: () => { ExtendedAssert.DeepEqual(expected, actual); });
        }

        [Fact]
        public void TwoListAreDeepEqualIfAllMembersAreEquals()
        {
            IReadOnlyList<MockGenericModel<string>> expected = CreateModelList(value: "expected");
            IReadOnlyList<MockGenericModel<string>> actual = CreateModelList(value: "expected");

            ExtendedAssert.DeepEqual(expected, actual);
        }

        [Fact]
        public void TwoObjectsAreNotDeepEqualIfAllMembersAreNotEquals()
        {
            IReadOnlyList<MockGenericModel<string>> expected = CreateModelList(value: "expected");
            IReadOnlyList<MockGenericModel<string>> actual = CreateModelList(value: "actual");

            Assert.Throws<EqualException>(testCode: () => { ExtendedAssert.DeepEqual(expected, actual); });
        }

        [Fact]
        public void TwoObjectsAreNotDeepEqualIfAnyNestedValueOfAnyMemberIsNotEqual()
        {
            MockGenericModel<string> expectedMember = CreateModel(value: "expected");
            expectedMember.NestedValue = new[] { "new nested value" };

            IReadOnlyList<MockGenericModel<string>> expected = new ReadOnlyCollection<MockGenericModel<string>>(new List<MockGenericModel<string>>() { expectedMember });
            IReadOnlyList<MockGenericModel<string>> actual = CreateModelList(value: "actual");

            Assert.Throws<EqualException>(testCode: () => { ExtendedAssert.DeepEqual(expected, actual); });
        }

        [Fact]
        public void ObjectsAreEqualIfTheyAreSameReference()
        {
            MockGenericModel<int> obj = new MockGenericModel<int>(value: 1);

            ExtendedAssert.AreEqual(left: obj, right: obj, this._equals);
        }

        [Fact]
        public void ObjectsAreEqualIfTheirNonReferencePartsAreEquals()
        {
            const int value = 1;
            MockGenericModel<int> left = new MockGenericModel<int>(value);
            MockGenericModel<int> right = new MockGenericModel<int>(value);

            ExtendedAssert.AreEqual(left, right, eq: (l, r) => l.Value.Equals(r.Value));
        }

        [Fact]
        public void ObjectsAreNotEqualIfLeftIsNull()
        {
            MockGenericModel<int> right = new MockGenericModel<int>(value: 1);

            Assert.Throws<TrueException>(testCode: () => ExtendedAssert.AreEqual(left: null, right, this._equals));
        }

        [Fact]
        public void ObjectsAreNotEqualIfRightIsNull()
        {
            MockGenericModel<int> left = new MockGenericModel<int>(value: 1);

            Assert.Throws<TrueException>(testCode: () => ExtendedAssert.AreEqual(left, right: null, this._equals));
        }

        [Fact]
        public void ObjectsAreNotEqualIfTheirNonReferencePartsAreNotEquals()
        {
            MockGenericModel<int> left = new MockGenericModel<int>(value: 1);
            MockGenericModel<int> right = new MockGenericModel<int>(value: 2);

            Assert.Throws<TrueException>(testCode: () => ExtendedAssert.AreEqual(left, right, this._equals));
        }

        [Fact]
        public void ObjectsAreSameIfTheyAreSameReference()
        {
            MockGenericModel<int> obj = new MockGenericModel<int>(value: 1);

            ExtendedAssert.Compare(left: obj, right: obj, this._compare, expected: 0);
        }

        [Fact]
        public void ObjectsAreSameIfTheirNonReferencePartsAreEquals()
        {
            const int value = 1;
            MockGenericModel<int> left = new MockGenericModel<int>(value);
            MockGenericModel<int> right = new MockGenericModel<int>(value);

            ExtendedAssert.Compare(left, right, this._compare, expected: 0);
        }

        [Fact]
        public void ObjectsAreNotSameIfLeftIsNull()
        {
            MockGenericModel<int> right = new MockGenericModel<int>(value: 1);

            ExtendedAssert.Compare(left: null, right, this._compare, expected: 1);
        }

        [Fact]
        public void ObjectsAreNotSameIfRightIsNull()
        {
            MockGenericModel<int> left = new MockGenericModel<int>(value: 1);

            ExtendedAssert.Compare(left, right: null, this._compare, expected: -1);
        }

        [Fact]
        public void ObjectsAreNotSameIfLeftNonReferenceIsLess()
        {
            MockGenericModel<int> left = new MockGenericModel<int>(value: 1);
            MockGenericModel<int> right = new MockGenericModel<int>(value: 2);

            ExtendedAssert.Compare(left, right, this._compare, expected: -1);
        }

        [Fact]
        public void ObjectsAreNotSameIfLeftNonReferenceIsBigger()
        {
            MockGenericModel<int> left = new MockGenericModel<int>(value: 2);
            MockGenericModel<int> right = new MockGenericModel<int>(value: 1);

            ExtendedAssert.Compare(left, right, this._compare, expected: 1);
        }
    }
}