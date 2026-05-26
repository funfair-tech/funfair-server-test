using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace FunFair.Test.Common.Mocks;

[DebuggerDisplay("{Level}: {Message}")]
public readonly record struct CapturedLogEntry(LogLevel Level, EventId EventId, string Message);
