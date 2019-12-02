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
            this.Output.WriteLine(message: "Hello World.");
        }
    }
}