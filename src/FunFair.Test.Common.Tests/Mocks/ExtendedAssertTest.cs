using System.Collections.Generic;
using System.Collections.ObjectModel;
using FunFair.Test.Common.Mocks;
using Xunit;
using Xunit.Sdk;

namespace FunFair.Test.Common.Tests.Mocks;

public sealed class ExtendedAssertTest : TestBase
{
    private static MockGenericModel<string> CreateModel(string value)
    {
        return new(value);
    }

    private static IReadOnlyList<MockGenericModel<string>> CreateModelList(string value)
    {
        return new ReadOnlyCollection<MockGenericModel<string>>(new List<MockGenericModel<string>> { CreateModel(value) });
    }

    [Fact]
    public void TwoListAreDeepEqualIfAllMembersAreEquals()
    {
        IReadOnlyList<MockGenericModel<string>> expectedResult = CreateModelList(value: "expected");
        IReadOnlyList<MockGenericModel<string>> actualResult = CreateModelList(value: "expected");

        ExtendedAssert.DeepEqual(expected: expectedResult, actual: actualResult);
    }

    [Fact]
    public void TwoObjectsAreDeepEqualIfAllValuesAreEquals()
    {
        MockGenericModel<string> expectedResult = CreateModel(value: "expected");
        MockGenericModel<string> actualResult = CreateModel(value: "expected");

        ExtendedAssert.DeepEqual(expected: expectedResult, actual: actualResult);
    }

    [Fact]
    public void TwoObjectsAreNotDeepEqualIfAllMembersAreNotEquals()
    {
        IReadOnlyList<MockGenericModel<string>> expected = CreateModelList(value: "expected");
        IReadOnlyList<MockGenericModel<string>> actual = CreateModelList(value: "actual");

        Assert.Throws<EqualException>(testCode: () => { ExtendedAssert.DeepEqual(expected: expected, actual: actual); });
    }

    [Fact]
    public void TwoObjectsAreNotDeepEqualIfAllValuesAreEquals()
    {
        MockGenericModel<string> expected = CreateModel(value: "expected");
        MockGenericModel<string> actual = CreateModel(value: "actual");

        Assert.Throws<EqualException>(testCode: () => { ExtendedAssert.DeepEqual(expected: expected, actual: actual); });
    }

    [Fact]
    public void TwoObjectsAreNotDeepEqualIfAnyNestedValueIsNotEqual()
    {
        MockGenericModel<string> expectedResult = CreateModel(value: "expected");
        expectedResult.NestedValue = new[]
                                     {
                                         "new nested value"
                                     };
        MockGenericModel<string> actualResult = CreateModel(value: "expected");

        Assert.Throws<EqualException>(testCode: () => ExtendedAssert.DeepEqual(expected: expectedResult, actual: actualResult));
    }

    [Fact]
    public void TwoObjectsAreNotDeepEqualIfAnyNestedValueOfAnyMemberIsNotEqual()
    {
        MockGenericModel<string> expectedMember = CreateModel(value: "expected");
        expectedMember.NestedValue = new[]
                                     {
                                         "new nested value"
                                     };

        IReadOnlyList<MockGenericModel<string>> expected = new ReadOnlyCollection<MockGenericModel<string>>(new List<MockGenericModel<string>> { expectedMember });
        IReadOnlyList<MockGenericModel<string>> actual = CreateModelList(value: "actual");

        Assert.Throws<EqualException>(testCode: () => { ExtendedAssert.DeepEqual(expected: expected, actual: actual); });
    }
}