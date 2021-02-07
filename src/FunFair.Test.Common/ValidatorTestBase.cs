using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Xunit;
using Xunit.Abstractions;

namespace FunFair.Test.Common
{
    /// <summary>
    ///     Base class for testing validators.
    /// </summary>
    /// <typeparam name="TValidator">The validator to test.</typeparam>
    /// <typeparam name="TObject">The object the validator tests</typeparam>
    public abstract class ValidatorTestBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
                                            TValidator, TObject> : LoggingTestBase
        where TValidator : AbstractValidator<TObject>, new()
    {
        private readonly TValidator _validator;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="output">Test output.</param>
        protected ValidatorTestBase(ITestOutputHelper output)
            : base(output)
        {
            this._validator = new TValidator();
        }

        /// <summary>
        ///     Validates the object.
        /// </summary>
        /// <param name="instance">The object to validate</param>
        /// <returns>Validation result.</returns>
        public ValidationResult Validate(TObject instance)
        {
            ValidationResult result = this._validator.Validate(instance);

            this.Dump(result);

            return result;
        }

        /// <summary>
        ///     Validates the object.
        /// </summary>
        /// <param name="instance">The object to validate</param>
        /// <param name="expectedErrorCount">The expected number of errors.</param>
        /// <returns>Validation result.</returns>
        public ValidationResult Validate(TObject instance, int expectedErrorCount)
        {
            ValidationResult result = this.Validate(instance);

            Assert.Equal(expected: expectedErrorCount, actual: result.Errors.Count);

            return result;
        }

        /// <summary>
        ///     Validates the object.
        /// </summary>
        /// <param name="instance">The object to validate</param>
        /// <param name="expectedErrorCount">The expected number of errors.</param>
        /// <param name="erroringProperty">The property expected to have errors.</param>
        /// <returns>Validation result.</returns>        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public ValidationResult Validate(TObject instance, int expectedErrorCount, string erroringProperty)
        {
            ValidationResult result = this.Validate(instance: instance, expectedErrorCount: expectedErrorCount);

            AssertOnlyNamedPropertyHasErrors(result: result, erroringProperty: erroringProperty);

            return result;
        }

        /// <summary>
        ///     Validates the object.
        /// </summary>
        /// <param name="instance">The object to validate</param>
        /// <param name="expectedErrorCount">The expected number of errors.</param>
        /// <param name="erroringProperties">The properties expected to have errors.</param>
        /// <returns>Validation result.</returns>        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        public ValidationResult Validate(TObject instance, int expectedErrorCount, params string[] erroringProperties)
        {
            ValidationResult result = this.Validate(instance: instance, expectedErrorCount: expectedErrorCount);

            AssertNamedPropertiesHaveErrors(result: result, erroringProperties: erroringProperties);

            return result;
        }

        /// <summary>
        ///     Creates an instance of an object that is valid.
        /// </summary>
        /// <returns>A valid object instance.</returns>
        protected abstract TObject CreateAValidObject();

        /// <summary>
        ///     Tests that all properties in the object pass validation.
        /// </summary>        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        protected void TestEverythingValid()
        {
            TObject itemToValidate = this.CreateAValidObject();

            this.Validate(instance: itemToValidate, expectedErrorCount: 0);
        }

        /// <summary>
        ///     Check that only the named property has errors.
        /// </summary>
        /// <param name="result">The validation result.</param>
        /// <param name="erroringProperty">The property expected to have errors.</param>
        protected static void AssertOnlyNamedPropertyHasErrors(ValidationResult result, string erroringProperty)
        {
            Assert.True(result.Errors.All(predicate: e => e.PropertyName == erroringProperty),
                        $"Should only have had errors in {erroringProperty}, but found errors in {string.Join(separator: ",", result.Errors.Select(selector: e => e.PropertyName).Distinct())}");
        }

        /// <summary>
        ///     Check that only the named property has errors.
        /// </summary>
        /// <param name="result">The validation result.</param>
        /// <param name="erroringProperty">The property expected to have errors.</param>        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        protected static void AssertNamedPropertyHasErrors(ValidationResult result, string erroringProperty)
        {
            Assert.True(result.Errors.Any(predicate: e => e.PropertyName == erroringProperty),
                        $"Should have had errors in {erroringProperty}, but not found found errors in {DumpPropertiesInError(result)}");
        }

        /// <summary>
        ///     Check that only the named property has errors.
        /// </summary>
        /// <param name="result">The validation result.</param>
        /// <param name="erroringProperties">The property expected to have errors.</param>
        protected static void AssertNamedPropertiesHaveErrors(ValidationResult result, params string[] erroringProperties)
        {
            Assert.NotEmpty(erroringProperties);

            bool hasUnexpectedErrors = result.Errors.All(predicate: error => erroringProperties.Contains(error.PropertyName));
            bool hasAllExpectedErrors = erroringProperties.All(predicate: error => result.Errors.Any(predicate: p => p.PropertyName == error));

            Assert.True(condition: hasUnexpectedErrors,
                        $"Should have had errors in {DumpExpectedPropertiesInError(erroringProperties)}, but not found found errors in {DumpPropertiesInError(result)}");

            Assert.True(condition: hasAllExpectedErrors,
                        $"Should have had errors in {DumpExpectedPropertiesInError(erroringProperties)}, but not found found errors in {DumpPropertiesInError(result)}");
        }

        /// <summary>
        ///     Builds the property name from a succession of parts.
        /// </summary>
        /// <param name="parts">The parts.</param>
        /// <returns>The property name.</returns>        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        protected static string MakePropertyName(params string[] parts)
        {
            return string.Join(separator: ".", value: parts);
        }

        /// <summary>
        ///     Outputs the validation results.
        /// </summary>
        /// <param name="result">The validation results</param>
        private void Dump(ValidationResult result)
        {
            if (!result.Errors.Any())
            {
                this.Output.WriteLine(message: "Validation Success");

                return;
            }

            this.Output.WriteLine($"Found {result.Errors.Count} errors:");

            foreach (ValidationFailure error in result.Errors.OrderBy(keySelector: e => e.PropertyName)
                                                      .ThenBy(keySelector: e => e.ErrorMessage))
            {
                this.Output.WriteLine($" * {error.PropertyName} : {error.ErrorMessage}");
            }
        }

        private static string DumpPropertiesInError(ValidationResult result)
        {
            return string.Join(separator: ", ",
                               result.Errors.Select(selector: e => e.PropertyName)
                                     .Distinct()
                                     .OrderBy(keySelector: x => x.ToUpperInvariant()));
        }

        private static string DumpExpectedPropertiesInError(string[] erroringProperties)
        {
            return string.Join(separator: ", ",
                               erroringProperties.Distinct()
                                                 .OrderBy(keySelector: x => x.ToUpperInvariant()));
        }

        /// <summary>
        ///     Checks that everything is valid
        /// </summary>
        [Fact]
        protected abstract void EverythingValid();
    }
}
