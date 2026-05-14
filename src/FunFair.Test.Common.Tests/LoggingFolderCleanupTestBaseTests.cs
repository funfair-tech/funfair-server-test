using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using FunFair.Test.Common.Tests.Extensions;
using FunFair.Test.Common.Tests.Mocks;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Internal;

namespace FunFair.Test.Common.Tests;

public sealed class LoggingFolderCleanupTestBaseTests : LoggingFolderCleanupTestBase
{
    public LoggingFolderCleanupTestBaseTests(ITestOutputHelper output)
        : base(output) { }

    [Fact]
    public void OutputOutputs()
    {
        DateTimeOffset now = TimeProvider.System.GetUtcNow();

        try
        {
            this.Output.WriteLine($"Hello World. It's {now}");
        }
        catch (Exception exception)
        {
            throw new FormatException(message: "Twit", innerException: exception);
        }
    }

    [Fact]
    public void LoggingOutputs()
    {
        ILogger<LoggingTestBaseTests> logger = this.GetTypedLogger<LoggingTestBaseTests>();

        DateTimeOffset now = TimeProvider.System.GetUtcNow();

        try
        {
            logger.LogHelloWorld(now);
        }
        catch (Exception exception)
        {
            throw new FormatException(message: "Twit", innerException: exception);
        }
    }

    [Fact]
    public void MakeFaker()
    {
        IReadOnlyList<ExampleObject> fake = MakeFake<ExampleObject>(
            rules: rules => rules.RuleFor(property: x => x.Name, setter: (f, _) => f.Company.Bs()),
            itemCount: 10
        );

        Assert.Equal(expected: 10, actual: fake.Count);

        fake.ForEach(item => this.Output.WriteLine($"* {item.Name}"));
    }

    [Fact]
    public void CanWriteToFolder()
    {
        string directory = this.CreateFolderInTempFolder(Guid.NewGuid().ToString());

        string filename = Path.Combine(path1: directory, path2: "temp-file.bin");

        byte[] source = "Hello World"u8.ToArray();
        Assert.False(File.Exists(filename), $"File {filename} already exists before creation");
        File.WriteAllBytes(path: filename, bytes: source);
        Assert.True(File.Exists(filename), $"File {filename} doesn't exist when it has just been written");
        byte[] written = File.ReadAllBytes(filename);

        Assert.Equal(expected: source, actual: written);
    }
}
