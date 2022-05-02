using System.Diagnostics;

namespace FunFair.Test.Common.Tests;

[DebuggerDisplay("Name: {Name}")]
public sealed record ExampleRecord(string Name);