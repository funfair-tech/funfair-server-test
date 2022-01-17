using System.Collections.Generic;
using System.Collections.ObjectModel;
using FunFair.Test.Common.Mocks;
using Xunit;
using Xunit.Sdk;

namespace FunFair.Test.Common.Tests.Mocks;

public sealed class ExtendedAssertTest : TestBase
{
    private static MockGenericModel2<string> CreateModel(string value)
    {
        return new(value);
    }

    private static IReadOnlyList<MockGenericModel2<string>> CreateModelList(string value)
    {
        return new ReadOnlyCollection<MockGenericModel2<string>>(new List<MockGenericModel2<string>> { CreateModel(value) });
    }

    [Fact]
    public void TwoListAreDeepEqualIfAllMembersAreEquals()
    {
        IReadOnlyList<MockGenericModel2<string>> expectedResult = CreateModelList(value: "expected");
        IReadOnlyList<MockGenericModel2<string>> actualResult = CreateModelList(value: "expected");

        ExtendedAssert.DeepEqual(expected: expectedResult, actual: actualResult);
    }

    [Fact]
    public void TwoObjectsAreDeepEqualIfAllValuesAreEquals()
    {
        MockGenericModel2<string> expectedResult = CreateModel(value: "expected");
        MockGenericModel2<string> actualResult = CreateModel(value: "expected");

        ExtendedAssert.DeepEqual(expected: expectedResult, actual: actualResult);
    }

    [Fact]
    public void TwoObjectsAreNotDeepEqualIfAllMembersAreNotEquals()
    {
        IReadOnlyList<MockGenericModel2<string>> expected = CreateModelList(value: "expected");
        IReadOnlyList<MockGenericModel2<string>> actual = CreateModelList(value: "actual");

        Assert.Throws<EqualException>(testCode: () => { ExtendedAssert.DeepEqual(expected: expected, actual: actual); });
    }

    [Fact]
    public void TwoObjectsAreNotDeepEqualIfAllValuesAreEquals()
    {
        MockGenericModel2<string> expected = CreateModel(value: "expected");
        MockGenericModel2<string> actual = CreateModel(value: "actual");

        Assert.Throws<EqualException>(testCode: () => { ExtendedAssert.DeepEqual(expected: expected, actual: actual); });
    }

    [Fact]
    public void TwoObjectsAreNotDeepEqualIfAnyNestedValueIsNotEqual()
    {
        MockGenericModel2<string> expectedResult = CreateModel(value: "expected");
        expectedResult.NestedValue = new[]
                                     {
                                         "new nested value"
                                     };
        MockGenericModel2<string> actualResult = CreateModel(value: "expected");

        Assert.Throws<EqualException>(testCode: () => ExtendedAssert.DeepEqual(expected: expectedResult, actual: actualResult));
    }

    [Fact]
    public void TwoObjectsAreNotDeepEqualIfAnyNestedValueOfAnyMemberIsNotEqual()
    {
        MockGenericModel2<string> expectedMember = CreateModel(value: "expected");
        expectedMember.NestedValue = new[]
                                     {
                                         "new nested value"
                                     };

        IReadOnlyList<MockGenericModel2<string>> expected = new ReadOnlyCollection<MockGenericModel2<string>>(new List<MockGenericModel2<string>> { expectedMember });
        IReadOnlyList<MockGenericModel2<string>> actual = CreateModelList(value: "actual");

        Assert.Throws<EqualException>(testCode: () => { ExtendedAssert.DeepEqual(expected: expected, actual: actual); });
    }
}