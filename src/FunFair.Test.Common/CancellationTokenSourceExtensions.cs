using System.Diagnostics.CodeAnalysis;

namespace FunFair.Test.Common;

[SuppressMessage(category: "Meziantou.Analyzer", checkId: "MA0042: Use CancelAsync", Justification = "Implementing the CancelAsync method it is complaining about!")]
public static partial class CancellationTokenSourceExtensions;
