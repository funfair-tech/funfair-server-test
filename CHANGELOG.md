# Changelog
All notable changes to this project will be documented in this file.

<!--
Please ADD ALL Changes to the UNRELASED SECTION and not a specific release
-->
## [Unreleased]
### Added
### Fixed
### Changed

<!-- 
Releases that have at least been deployed to staging, BUT NOT necessarily released to live.  Changes should be moved from [Unreleased] into here as they are merged into the appropriate release branch
-->
## [1.0.1] 2019-11-27
### Changed
* Made ValidatorTestBase expose a Validate method rather than exposing the validator itself
* Added a ServiceProvider method to IntegrationTest base to allow access to the registered service provider
* Simplified the LoggingTestBase class so that it no longer makes gratuitous use of reflection.

## [1.0.0] 2019-11-22
### Added
- Initial version
