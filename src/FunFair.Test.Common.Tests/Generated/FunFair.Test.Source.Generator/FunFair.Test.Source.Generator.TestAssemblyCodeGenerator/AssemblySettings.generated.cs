using System.Diagnostics.CodeAnalysis;

[assembly: ExcludeFromCodeCoverage]

[assembly: SuppressMessage(category: "Philips.CodeAnalysis.MaintainabilityAnalyzers", checkId: "PH2140: Avoid ExcludeFromCodeCoverage", Justification = "This is a unit test assembly - no need for coverage of the test code itself")]
[assembly: SuppressMessage(category: "Philips.CodeAnalysis.DuplicateCodeAnalyzer", checkId: "PH2071: Duplicate code", Justification = "This is a unit test assembly - Highly likely there will be duplicate code")]
[assembly: SuppressMessage(category: "Microsoft.Performance", checkId: "CA2254: Use a fixed template in logging", Justification = "This is a unit test assembly - Although nice to have a fixed template, its not needed here")]
