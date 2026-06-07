using System;
using FunFair.Test.Common.Helpers;
using Xunit;

namespace FunFair.Test.Common.Tests.Helpers;

public sealed class EnvironmentVariableScopeTests : TestBase
{
    private const string TEST_VARIABLE_NAME = "FUNFAIR_TEST_ENV_VAR_SCOPE_TEST";

    [Fact]
    public void SetsEnvironmentVariableForDurationOfScope()
    {
        const string expectedValue = "test-value";

        Environment.SetEnvironmentVariable(variable: TEST_VARIABLE_NAME, value: null);

        using (new EnvironmentVariableScope(variableName: TEST_VARIABLE_NAME, value: expectedValue))
        {
            Assert.Equal(expected: expectedValue, actual: Environment.GetEnvironmentVariable(TEST_VARIABLE_NAME));
        }
    }

    [Fact]
    public void RestoresOriginalValueAfterDispose()
    {
        const string originalValue = "original-value";
        const string scopeValue = "scope-value";

        Environment.SetEnvironmentVariable(variable: TEST_VARIABLE_NAME, value: originalValue);

        using (new EnvironmentVariableScope(variableName: TEST_VARIABLE_NAME, value: scopeValue))
        {
            // inside scope: value is scopeValue
        }

        Assert.Equal(expected: originalValue, actual: Environment.GetEnvironmentVariable(TEST_VARIABLE_NAME));
    }

    [Fact]
    public void RestoresNullWhenOriginalValueWasNotSet()
    {
        const string scopeValue = "scope-value";

        Environment.SetEnvironmentVariable(variable: TEST_VARIABLE_NAME, value: null);

        using (new EnvironmentVariableScope(variableName: TEST_VARIABLE_NAME, value: scopeValue))
        {
            // inside scope: value is scopeValue
        }

        Assert.Null(Environment.GetEnvironmentVariable(TEST_VARIABLE_NAME));
    }

    [Fact]
    public void CanSetNullValueWithinScope()
    {
        const string originalValue = "original-value";

        Environment.SetEnvironmentVariable(variable: TEST_VARIABLE_NAME, value: originalValue);

        using (new EnvironmentVariableScope(variableName: TEST_VARIABLE_NAME, value: null))
        {
            Assert.Null(Environment.GetEnvironmentVariable(TEST_VARIABLE_NAME));
        }

        Assert.Equal(expected: originalValue, actual: Environment.GetEnvironmentVariable(TEST_VARIABLE_NAME));
    }

    [Fact]
    public void DisposingTwiceDoesNotThrow()
    {
        Environment.SetEnvironmentVariable(variable: TEST_VARIABLE_NAME, value: null);

        EnvironmentVariableScope scope = new(variableName: TEST_VARIABLE_NAME, value: "some-value");
        scope.Dispose();
        scope.Dispose();
    }
}
