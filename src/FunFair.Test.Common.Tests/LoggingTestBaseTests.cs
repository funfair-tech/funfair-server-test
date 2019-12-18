using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Tests
{
    public sealed class LoggingTestBaseTests : LoggingTestBase
    {
        public LoggingTestBaseTests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        [SuppressMessage("FunFair.CodeAnalysis", "FFS0005:Avoid DateTimeOffset.UtcNow", Justification = "Unit test")]
        public void OutputOutputs()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            this.Output.WriteLine($"Hello World. It's {now}");
        }
    }
}