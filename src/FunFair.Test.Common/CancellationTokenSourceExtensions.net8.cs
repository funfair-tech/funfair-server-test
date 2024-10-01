#if NET8_0_OR_GREATER
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FunFair.Test.Common;

[SuppressMessage(category: "Meziantou.Analyzer", checkId: "MA0042: Use CancelAsync", Justification = "Implementing the CancelAsync method it is complaining about!")]
public static partial class CancellationTokenSourceExtensions
{
    public static Task CancelAsync(this CancellationTokenSource cancellationTokenSource, bool throwOnFirstException)
    {
        if (throwOnFirstException)
        {
            Debug.WriteLine("throwOnFirstException is ignored");
        }

        return cancellationTokenSource.CancelAsync();
    }
}
#endif