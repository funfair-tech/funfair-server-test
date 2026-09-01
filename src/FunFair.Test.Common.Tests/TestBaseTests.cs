using System.Threading;
using System.Threading.Tasks;
using FunFair.Test.Common.Mocks;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Sdk;

namespace FunFair.Test.Common.Tests;

public sealed class TestBaseTests : TestBase
{
    [Fact]
    public void AssertReallyNotNullReturnsValueForNonNullStruct()
    {
        int? value = 5;

        int result = AssertReallyNotNull(value);

        Assert.Equal(expected: 5, actual: result);
    }

    [Fact]
    public void AssertReallyNotNullThrowsForNullStruct()
    {
        int? value = null;

        Assert.Throws<NotNullException>(testCode: () => AssertReallyNotNull(value));
    }

    [Fact]
    public void FormatValueReturnsToStringWhenOverridden()
    {
        ExampleRecord value = new(Name: "test");

        string result = FormatValue(value);

        Assert.Equal(expected: value.ToString(), actual: result);
    }

    [Fact]
    public void FormatValueThrowsWhenToStringNotOverridden()
    {
        ExampleObject value = new() { Name = "test" };

        Assert.Throws<FalseException>(testCode: () => FormatValue(value));
    }

    [Fact]
    public void GetSubstituteWithTwoInterfacesReturnsInstanceImplementingBoth()
    {
        ITestInterface result = GetSubstitute<ITestInterface, ITestInterface2>();

        Assert.IsAssignableFrom<ITestInterface2>(result);
    }

    [Fact]
    public void GetSubstituteWithThreeInterfacesReturnsInstanceImplementingAll()
    {
        ITestInterface result = GetSubstitute<ITestInterface, ITestInterface2, ITestInterface3>();

        Assert.IsAssignableFrom<ITestInterface2>(result);
        Assert.IsAssignableFrom<ITestInterface3>(result);
    }

    [Fact]
    public void CreateCancellationTokenSourceLinksExternalToken()
    {
        using CancellationTokenSource externalSource = new();
        using CancellationTokenSource linked = this.CreateCancellationTokenSource(externalSource.Token);

        Assert.False(
            condition: linked.IsCancellationRequested,
            userMessage: "Linked token should not be cancelled yet"
        );

        externalSource.Cancel();

        Assert.True(
            condition: linked.IsCancellationRequested,
            userMessage: "Linked token should be cancelled after external source is cancelled"
        );
    }

    [Fact]
    public async Task FromOptionalResultAsyncReturnsProvidedValueAsync()
    {
        ExampleObject value = new() { Name = "test" };

        ExampleObject? result = await FromOptionalResultAsync(value);

        Assert.Same(expected: value, actual: result);
    }

    [Fact]
    public async Task FromOptionalResultAsyncReturnsNullWhenGivenNullAsync()
    {
        ExampleObject? result = await FromOptionalResultAsync<ExampleObject>(null);

        Assert.Null(result);
    }

    [Fact]
    public async Task NullResultAsyncReturnsNullAsync()
    {
        ExampleObject? result = await NullResultAsync<ExampleObject>();

        Assert.Null(result);
    }

    [Fact]
    public void GetTypedLoggerReturnsSubstituteLogger()
    {
        ILogger<TestBaseTests> logger = this.GetTypedLogger<TestBaseTests>();

        Assert.NotNull(logger);
    }
}
