using System.Threading;

namespace FunFair.Test.Common.Logging;

internal static class TestOutputLocking
{
    public static SemaphoreSlim TestLock { get; } = new(1);
}