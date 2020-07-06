# Changelog
All notable changes to this project will be documented in this file.

<!--
Please ADD ALL Changes to the UNRELASED SECTION and not a specific release
-->
## [Unreleased]
### Added
### Fixed
### Changed
- FF-1429 - Updated FunFair.CodeAnalysis to 1.7.0.347
- FF-1429 - Updated FunFair.CodeAnalysis to 1.6.0.343
- FF-1429 - Updated SonarAnalyzer.CSharp to 8.9.0.19135
- FF-1429 - Updated FunFair.CodeAnalysis to 1.5.0.314
### Removed

<!--
Releases that have at least been deployed to staging, BUT NOT necessarily released to live.  Changes should be moved from [Unreleased] into here as they are merged into the appropriate release branch
-->

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








