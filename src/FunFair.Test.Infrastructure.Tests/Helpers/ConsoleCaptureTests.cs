using System;
using System.IO;
using FunFair.Test.Common;
using FunFair.Test.Infrastructure.Helpers;
using Xunit;

namespace FunFair.Test.Infrastructure.Tests.Helpers;

public sealed class ConsoleCaptureTests : TestBase
{
    [Fact]
    public void CapturesStdOutWithinScope()
    {
        const string message = "hello stdout";

        using (ConsoleCapture capture = new())
        {
            Console.Write(message);
            Assert.Equal(expected: message, actual: capture.StdOut);
        }
    }

    [Fact]
    public void CapturesStdErrWithinScope()
    {
        const string message = "hello stderr";

        using (ConsoleCapture capture = new())
        {
            Console.Error.Write(message);
            Assert.Equal(expected: message, actual: capture.StdErr);
        }
    }

    [Fact]
    public void StdOutIsEmptyWhenNothingWritten()
    {
        using ConsoleCapture capture = new();
        Assert.Equal(expected: string.Empty, actual: capture.StdOut);
    }

    [Fact]
    public void StdErrIsEmptyWhenNothingWritten()
    {
        using ConsoleCapture capture = new();
        Assert.Equal(expected: string.Empty, actual: capture.StdErr);
    }

    [Fact]
    public void RestoresConsoleOutAfterDispose()
    {
        TextWriter originalOut = Console.Out;

        ConsoleCapture capture = new();
        capture.Dispose();

        Assert.Same(expected: originalOut, actual: Console.Out);
    }

    [Fact]
    public void RestoresConsoleErrorAfterDispose()
    {
        TextWriter originalError = Console.Error;

        ConsoleCapture capture = new();
        capture.Dispose();

        Assert.Same(expected: originalError, actual: Console.Error);
    }

    [Fact]
    public void DisposingTwiceDoesNotThrow()
    {
        ConsoleCapture capture = new();
        capture.Dispose();
        capture.Dispose();
    }
}
