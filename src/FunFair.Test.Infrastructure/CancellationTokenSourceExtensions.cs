using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace FunFair.Test.Infrastructure;

[SuppressMessage(
    category: "Meziantou.Analyzer",
    checkId: "MA0042: Use CancelAsync",
    Justification = "Implementing the CancelAsync method it is complaining about!"
)]
public static class CancellationTokenSourceExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Task CancelAsync(this CancellationTokenSource cancellationTokenSource, bool throwOnFirstException)
    {
        LogThrowOnFirstException(throwOnFirstException);

        return cancellationTokenSource.CancelAsync();
    }

    [Conditional("DEBUG")]
    private static void LogThrowOnFirstException(bool throwOnFirstException)
    {
        if (throwOnFirstException)
        {
            Debug.WriteLine("throwOnFirstException is ignored");
        }
    }
}
