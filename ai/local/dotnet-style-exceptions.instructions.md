# .NET Style Exceptions

[Back to Local Instructions Index](index.md)

Rider's code inspection surfaces some findings this repo deliberately does not act on. Recorded here so future work does not re-propose the same fix.

`src/FunFair.Test.slnx.DotSettings` already sets solution-wide Rider inspection severities (e.g. `ConvertToUsingDeclaration` and `ConvertConstructorToMemberInitializers` are set to `DO_NOT_SHOW`), so a per-inspection severity override is an available mechanism, not a new one, for the `MockBase<T>` and `ExcludeFromCodeCoverage` entries below (their IDs are not yet identified). It has not been used here: it is solution-wide, silencing the inspection for every file rather than the two specific members it's justified for, so it needs the repo owner's explicit sign-off before use, same as any other new suppression under this repo's Warning Suppression rules. This markdown note is the interim record until that happens.

## Never Convert to Primary Constructors

Do not convert a `class` or `struct` to use a primary constructor, regardless of what Rider or `/simplify` suggests. This applies repo-wide, not just to existing code. Unlike the other entries below, this is a deliberate team style preference, not a workaround for an analyzer false positive or bug; it has no re-evaluation trigger and does not expire.

## Extension Blocks (C# 14) Not Yet Analyzer-Safe

Do not convert extension methods to the new C# 14 `extension(...)` block syntax in this repo. A scratch test (an `extension(Summary summary) { ... }` block mirroring `FunFair.Test.Common/SummaryExtensions.cs`) showed the current analyzer stack misparses it:

- `Do not nest type` - misreads the `extension(...)` block as a nested type declaration.
- `Make 'X' method static` - does not recognise the block's receiver parameter, flags the extension methods as needing to be static.

Under `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` this breaks the build. Re-evaluate once the analyzer stack (Roslynator, Philips.CodeAnalysis, or whichever is responsible) ships C# 14 extension-block support.

## `MockBase<T>` Internal/Sealed Suggestion Is a False Positive

Rider flags `FunFair.Test.Infrastructure/Mocks/MockBase.cs`'s `MockBase<T>` with "must be internal" and "must be sealed". Do not apply either:

- `sealed` is impossible: the class is `abstract`, and `sealed abstract` is not legal C#.
- `internal` is already deliberately rejected: the type is consumed as a base class across assembly boundaries (e.g. `FunFair.Test.Common.Mocks/MockExampleObject.cs` and `FunFair.Test.Infrastructure.Tests/Mocks/MockExampleObject.cs` both derive from it from different projects than where it's declared). The class already carries `[SuppressMessage]` attributes for the equivalent `FunFair.CodeAnalysis` checks (FFS0029/FFS0030 "Should be internal") with `Justification = "Infrastructure"`.

## `ExcludeFromCodeCoverage` Suggestion Is a False Positive on Test-Infrastructure Assemblies

As with `MockBase<T>` above, Rider's "Avoid the ExcludeFromCodeCoverage attribute" finding does not apply to `FunFair.Test.Common/AssemblySettings.cs` and `FunFair.Test.Infrastructure/AssemblySettings.cs`'s `[assembly: ExcludeFromCodeCoverage]`, though for a different reason: both assemblies are test infrastructure, not coverage-ratchet targets, and both already carry an explicit `[assembly: SuppressMessage(category: "Philips.CodeAnalysis.MaintainabilityAnalyzers", checkId: "PH2140: Avoid ExcludeFromCodeCoverage", ...)]` with justification.
