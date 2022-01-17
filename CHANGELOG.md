# Changelog
All notable changes to this project will be documented in this file.

<!--
Please ADD ALL Changes to the UNRELEASED SECTION and not a specific release
-->
## [Unreleased]
### Added
### Fixed
### Changed
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.34.0.42011
- Tightened up the checks for RegisteredService so that it will refuse proxy objects from castle (NSubstitute)
### Removed

<!--
Releases that have at least been deployed to staging, BUT NOT necessarily released to live.  Changes should be moved from [Unreleased] into here as they are merged into the appropriate release branch
-->
## [5.8.0] - 2022-01-17
### Added
- Dependency Injection Test Base class.
### Changed
- FF-1429 - Updated Philips.CodeAnalysis.DuplicateCodeAnalyzer to 1.1.5
- FF-1429 - Updated Philips.CodeAnalysis.DuplicateCodeAnalyzer to 1.1.6

## [5.7.3] - 2021-12-17
### Changed
- FF-3881 - Updated DotNet SDK to 6.0.101

## [5.7.2] - 2021-12-15
### Changed
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.33.0.40503
- FF-1429 - Updated FluentValidation.AspNetCore to 10.3.6

## [5.7.0] - 2021-12-03
### Changed
- FF-1429 - Updated Microsoft.VisualStudio.Threading.Analyzers to 17.0.64
- FF-1429 - Updated To DotNet SDK 5.0.403
- FF-1429 - Updated NSubstitute.Analyzers.CSharp to 1.0.15
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.32.0.39516
- FF-1429 - Updated Roslynator.Analyzers to 3.3.0
- FF-1429 - Updated FluentValidation.AspNetCore to 10.3.5
- FF-1429 - Updated FunFair.CodeAnalysis to 5.7.3.1052
- FF-3856 - Updated to DotNet 6.0 with DotNet 5.0 fallback

## [5.6.2] - 2021-11-05
### Removed
- Redundant dependencies

## [5.6.1] - 2021-11-04
### Changed
- FF-1429 - Updated Microsoft.VisualStudio.Threading.Analyzers to 17.0.63
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.30.0.37606
- FF-1429 - Updated FluentValidation.AspNetCore to 10.3.4
- FF-1429 - Updated Microsoft.NET.Test.Sdk to 17.0.0
- FF-1429 - Updated FunFair.CodeAnalysis to 5.6.0.960
- FF-1429 - Updated Dapper to 2.0.123

## [5.6.0] - 2021-08-24
### Added
- Additional asserts in base classes for type converters - as it makes no sense for them to convert null - as input
  parameters are not nullable.
### Changed
- FF-1429 - Updated FunFair.CodeAnalysis to 5.2.5.870
- FF-1429 - Updated FunFair.CodeAnalysis to 5.3.0.879
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.27.0.35380
- FF-1429 - Updated Microsoft.NET.Test.Sdk to 16.11.0
- FF-1429 - Updated Roslynator.Analyzers to 3.2.2
- FF-1429 - Updated FluentValidation.AspNetCore to 10.3.1

## [5.5.0] - 2021-07-20
### Changed
- FF-1429 - Updated FunFair.CodeAnalysis to 5.2.1.809
- FF-1429 - Updated FluentValidation.AspNetCore to 10.3.0
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.26.0.34506
- FF-1429 - Updated FunFair.CodeAnalysis to 5.2.3.837
- FF-1429 - Updated coverlet to 3.1.0
- FF-1429 - Updated FunFair.CodeAnalysis to 5.2.4.854

## [5.4.0] - 2021-07-02
### Added
- Base class for testing Model Binders
### Changed
- FF-1429 - Updated FluentValidation.AspNetCore to 10.0.2
- FF-1429 - Updated FluentValidation.AspNetCore to 10.0.3
- FF-1429 - Updated FluentValidation.AspNetCore to 10.0.4
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.21.0.30542
- FF-1429 - Updated FluentValidation.AspNetCore to 10.1.0
- FF-1429 - Updated Dapper to 2.0.90
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.22.0.31243
- FF-1429 - Updated FunFair.CodeAnalysis to 5.2.0.740
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.23.0.32424
- FF-1429 - Updated Microsoft.NET.Test.Sdk to 16.10.0
- FF-1429 - Updated Microsoft.VisualStudio.Threading.Analyzers to 16.10.56
- FF-1429 - Updated FluentValidation.AspNetCore to 10.2.0
- FF-1429 - Updated FluentValidation.AspNetCore to 10.2.1
- FF-1429 - Updated FluentValidation.AspNetCore to 10.2.2
- FF-1429 - Updated FluentValidation.AspNetCore to 10.2.3
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.24.0.32949
- FF-1429 - Updated Roslynator.Analyzers to 3.2.0
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.25.0.33663

## [5.3.0] - 2021-04-06
### Changed
- FF-1429 - Updated Microsoft.NET.Test.Sdk to 16.9.4
- FF-1429 - Updated FluentValidation.AspNetCore to 9.5.4
- FF-1429 - Updated FluentValidation.AspNetCore to 10.0.0

## [5.2.0] - 2021-03-19
### Added
- Added GetLogger and GetSubstitute to TestBase so can call them in the same way as GetService etc in higher level objects
### Changed
- FF-1429 - Updated FluentValidation.AspNetCore to 9.5.3
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.20.0.28934

## [5.1.2] - 2021-03-12
### Added
- Additional test method in SqlTypeMapperTestsBase
### Changed
- FF-1429 - Updated AsyncFixer to 1.4.1
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.17.0.26580
- FF-1429 - Updated AsyncFixer to 1.5.1
- FF-1429 - Updated Roslynator.Analyzers to 3.1.0
- FF-1429 - Updated FluentValidation.AspNetCore to 9.5.0
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.18.0.27296
- FF-1429 - Updated FunFair.CodeAnalysis to 5.1.0.658
- FF-1429 - Updated FluentValidation.AspNetCore to 9.5.1
- FF-1429 - Updated coverlet to 3.0.3
- FF-1429 - Updated Microsoft.NET.Test.Sdk to 16.9.1
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.19.0.28253
- FF-1429 - Updated Microsoft.VisualStudio.Threading.Analyzers to 16.9.54
- FF-1429 - Updated FluentValidation.AspNetCore to 9.5.2
- FF-1429 - Updated Microsoft.VisualStudio.Threading.Analyzers to 16.9.60

## [5.1.1] - 2021-01-18
### Added
- added a `JsonConverterStructTestBase`
### Changed
- FF-1429 - Updated AsyncFixer to 1.4.0

## [5.1.0] - 2021-01-18
### Added
- added a `TypeConverterTestBase`
### Changed
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.16.0.25740
- FF-1429 - Updated FunFair.CodeAnalysis to 5.0.0.619
- FF-1429 - Updated FluentValidation.AspNetCore to 9.4.0

## [5.0.0] - 2020-12-18
### Changed
- FF-1429 - Updated Dapper to 2.0.78
- FF-1429 - Updated Microsoft.Extensions to 5.0.0
- FF-1429 - Updated FluentValidation.AspNetCore to 9.3.0
- FF-1429 - Updated NSubstitute.Analyzers.CSharp to 1.0.14
- FF-1429 - Updated Microsoft.NET.Test.Sdk to 16.8.0
- FF-1429 - Updated Microsoft.VisualStudio.Threading.Analyzers to 16.8.55
- FF-1429 - Updated FunFair.CodeAnalysis to 1.15.0.542
- FF-1429 - Updated Microsoft.CodeAnalysis.FxCopAnalyzers to 3.3.1
- FF-1429 - Updated Microsoft.VisualStudio.Threading.Analyzers to 16.8.51
- FF-1429 - Updated Microsoft.VisualStudio.Threading.Analyzers to 16.8.50
- FF-1429 - Updated FunFair.CodeAnalysis to 1.15.0.518
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.15.0.24505
- FF-1429 - Updated Microsoft.NET.Test.Sdk to 16.8.3
- FF-3198 - Update all the .NET components to .NET 5.0.101

## [1.14.0] 2020-10-14
### Changed
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.14.0.22654
- FF-1429 - Updated FunFair.CodeAnalysis to 1.14.0.468
- FF-1429 - Updated FunFair.CodeAnalysis to 1.13.0.452
- FF-1429 - Updated FunFair.CodeAnalysis to 1.12.0.445
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.13.1.21947
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.13.0.21683
- FF-2930 - Updated to .net core 3.1.403

## [1.13.0] 2020-09-10
### Added
- Additional analyser for checking usage of .ToString from object when it hasn't been overriden
- Formatting helper.

## [1.12.0] 2020-09-09
### Changed
- FF-2830 - Update all the .NET components to .NET Core 3.1.402
- FF-1429 - Updated FunFair.CodeAnalysis to 1.11.0.424
- FF-1429 - Updated Roslynator.Analyzers to 3.0.0
- FF-1429 - Updated Microsoft.VisualStudio.Threading.Analyzers to 16.7.56
- FF-1429 - Updated FunFair.CodeAnalysis to 1.10.0.414

## [1.11.3] 2020-09-02
### Added
- Added overloads to MockCreateClientWithResponse that allows response headers to be specified.

## [1.11.2] 2020-08-28
### Added
- Additional asserts that should really be part of xunit

## [1.11.1] 2020-08-17
### Changed
- Converted TestHttpClient into HttpClientFactoryExtensionsTests so that we don't need to cover all the methods that needed to create a client with IDisposable suppressions

## [1.11.0] 2020-08-12
### Changed
- FF-1429 - Updated Microsoft.CodeAnalysis.FxCopAnalyzers to 3.3.0
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.11.0.20529
- FF-1429 - Updated FluentValidation.AspNetCore to 9.1.2
- FF-1429 - Updated FluentValidation.AspNetCore to 9.1.1
- FF-1429 - Updated FluentValidation.AspNetCore to 9.1.0
- FF-1429 - Updated Microsoft.NET.Test.Sdk to 16.7.0
- FF-1429 - Updated xunit.runner.visualstudio to 2.4.3
- FF-1429 - Updated Microsoft.VisualStudio.Threading.Analyzers to 16.7.54
- FF-2759 - Updated to .net core 3.1.401

## [1.10.1] 2020-07-30
### Changed
- FF-2597 - Added not received extension method for logger testing

## [1.10.0] 2020-07-29
### Changed
- FF-2597 - Updated mock logger
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.10.0.19839

## [1.9.0] 2020-07-20
### Added
- Overloads for the HttpClient to take serialization options
### Changed
- FF-2652 - Update all the .NET components to .NET Core 3.1.302
- FF-1429 - Updated FunFair.CodeAnalysis to 1.8.0.375

## [1.8.2] 2020-07-15
### Changed
- FF-1429 - Updated FunFair.CodeAnalysis to 1.7.2.364
- Updated Fluent Validation to 9.0.1

## [1.8.1] 2020-07-08
### Added
- FF-2601 - Can return an HttpClient containing a fake message handler which can be configured to respond as required
### Changed
- FF-1429 - Updated FunFair.CodeAnalysis to 1.7.1.352
- FF-1429 - Updated FunFair.CodeAnalysis to 1.7.0.347
- FF-1429 - Updated FunFair.CodeAnalysis to 1.6.0.343
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.9.0.19135
- FF-1429 - Updated FunFair.CodeAnalysis to 1.5.0.314

## [1.8.0] 2020-06-18
### Changed
- FF-2488 - Updated packages and global.json to net core 3.1.301
- FF-1429 - Updated NSubstitute to 4.2.2

## [1.7.1] 2020-06-08
### Changed
- FF-1429 - Updated xunit.runner.visualstudio to 2.4.2
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.8.0.18411
- FF-1429 - Updated AsyncFixer to 1.3.0

## [1.7.0] 2020-05-13
### Changed
- FF-2386 - Update all the .NET components to .NET Core 3.1.202

## [1.6.2] 2020-05-05
### Added
- Base class for unit testing custom SqlTypeMappers.

## [1.6.1] 2020-04-27
### Changed
- FF-2295 - removed iequatable from jsonconvertertestbase

### Added
## [1.6.0] 2020-03-30
### Added

## [1.5.2] 2020-03-15
### Added
- Base classes for IComparable and IEquatable objects

## [1.5.1] 2020-02-27
### Added
- FF-821 - Support for ignoring unused variables in tests that are there to make the test work (e.g. when testing properties are accessed when using NSubstitute.

## [1.5.0] 2020-02-24
### Changed
- FF-1910 - updated to net core sdk 3.1.102

## [1.4.0] 2020-02-11
### Added
- Enforcement of Microsoft.VisualStudio.Threading.Analyzers

## [1.3.2] 2020-02-07
### Fixed
- Untimely disposal of Logging infrastructure... before its actually finished being used.

## [1.3.1] 2020-01-17
### Changed
- Updated to .net core sdk 3.1.101

## [1.3.0] 2019-12-10
### Added
- FF-1679 - Move common test base classes and infrastructure into Test.Common

## [1.2.0] 2019-12-06
- FF-1258 - Updated to .net core 3.1.100

## [1.1.0] 2019-12-02
### Changed
- ValidatorTestBase to expose more validate methods and deprecate the Dump method, as that is embedded in all validates.

## [1.0.1] 2019-11-27
### Changed
* Made ValidatorTestBase expose a Validate method rather than exposing the validator itself
* Added a ServiceProvider method to IntegrationTest base to allow access to the registered service provider
* Simplified the LoggingTestBase class so that it no longer makes gratuitous use of reflection.

## [1.0.0] 2019-11-22
### Added
- Initial version