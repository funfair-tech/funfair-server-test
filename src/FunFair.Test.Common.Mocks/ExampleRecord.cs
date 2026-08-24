using System.Diagnostics;

namespace FunFair.Test.Common.Mocks;

[DebuggerDisplay("Name: {Name}")]
public sealed record ExampleRecord(string Name);
