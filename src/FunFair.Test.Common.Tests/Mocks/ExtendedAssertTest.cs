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
            return new ReadOnlyCollection<MockGenericModel<string>>(new List<MockGenericModel<string>> {CreateModel(value)});
        }

        [Fact]
        public void TwoListAreDeepEqualIfAllMembersAreEquals()
        {
            IReadOnlyList<MockGenericModel<string>> expected = CreateModelList(value: "expected");
            IReadOnlyList<MockGenericModel<string>> actual = CreateModelList(value: "expected");

            ExtendedAssert.DeepEqual(expected, actual);
        }

        [Fact]
        public void TwoObjectsAreDeepEqualIfAllValuesAreEquals()
        {
            MockGenericModel<string> expected = CreateModel(value: "expected");
            MockGenericModel<string> actual = CreateModel(value: "expected");

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
            expected.NestedValue = new[] {"new nested value"};
            MockGenericModel<string> actual = CreateModel(value: "expected");

            Assert.Throws<EqualException>(testCode: () => { ExtendedAssert.DeepEqual(expected, actual); });
        }

        [Fact]
        public void TwoObjectsAreNotDeepEqualIfAnyNestedValueOfAnyMemberIsNotEqual()
        {
            MockGenericModel<string> expectedMember = CreateModel(value: "expected");
            expectedMember.NestedValue = new[] {"new nested value"};

            IReadOnlyList<MockGenericModel<string>> expected = new ReadOnlyCollection<MockGenericModel<string>>(new List<MockGenericModel<string>> {expectedMember});
            IReadOnlyList<MockGenericModel<string>> actual = CreateModelList(value: "actual");

            Assert.Throws<EqualException>(testCode: () => { ExtendedAssert.DeepEqual(expected, actual); });
        }
    }
}