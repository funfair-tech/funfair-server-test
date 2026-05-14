using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FunFair.Test.Common;

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
