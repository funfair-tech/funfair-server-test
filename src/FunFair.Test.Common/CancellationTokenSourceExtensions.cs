using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FunFair.Test.Common;

[SuppressMessage(category: "Meziantou.Analyzer", checkId: "MA0042: Use CancelAsync", Justification = "Implementing the CancelAsync method it is complaining about!")]
public static class CancellationTokenSourceExtensions
{
#if NET8_0_OR_GREATER
    public static Task CancelAsync(this CancellationTokenSource cancellationTokenSource, bool throwOnFirstException)
    {
        cancellationTokenSource.CancelAsync();

        return Task.CompletedTask;
    }
#else
    public static Task CancelAsync(this CancellationTokenSource cancellationTokenSource)
    {
        cancellationTokenSource.Cancel();

        return Task.CompletedTask;
    }

    public static Task CancelAsync(this CancellationTokenSource cancellationTokenSource, bool throwOnFirstException)
    {
        cancellationTokenSource.Cancel(throwOnFirstException);

        return Task.CompletedTask;
    }
#endif
}