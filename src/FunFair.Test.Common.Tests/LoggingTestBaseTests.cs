using System;
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
        public void OutputOutputs()
        {
            DateTimeOffset now = DateTimeOffset.Now;

            this.Output.WriteLine($"Hello World. It's {now}");
        }
    }
}