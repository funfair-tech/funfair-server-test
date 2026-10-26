using System;
using System.Collections.Generic;
using Xunit;

namespace FunFair.Test.Common.Tests;

public sealed class DispatcherCaseDataTests : TestBase
{
    [Fact]
    public void CaseNamesTheCaseAfterTheCalledMethod()
    {
        (string name, Action<List<int>> _) = DispatcherCaseData.Case<List<int>>(static list => list.Clear());

        Assert.Equal(expected: "Clear", actual: name);
    }

    [Fact]
    public void CaseThrowsWhenTheActionMakesNoCall()
    {
        ArgumentException exception = Assert.Throws<ArgumentException>(testCode: static () =>
            DispatcherCaseData.Case<string>(static _ => { })
        );

        Assert.Equal(expected: "expressionText", actual: exception.ParamName);
    }
}
