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
    public abstract class ValidatorTestBase<TValidator, TObject> : LoggingTestBase
        where TValidator : AbstractValidator<TObject>, new()
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="output">Test output.</param>
        protected ValidatorTestBase(ITestOutputHelper output)
            : base(output)
        {
            this._validator = new TValidator();
        }

        private readonly TValidator _validator;

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

            Assert.Equal(expectedErrorCount, result.Errors.Count);

            return result;
        }

        /// <summary>
        ///     Validates the object.
        /// </summary>
        /// <param name="instance">The object to validate</param>
        /// <param name="expectedErrorCount">The expected number of errors.</param>
        /// <param name="erroringProperty">The property expected to have errors.</param>
        /// <returns>Validation result.</returns>
        public ValidationResult Validate(TObject instance, int expectedErrorCount, string erroringProperty)
        {
            ValidationResult result = this.Validate(instance, expectedErrorCount);

            AssertOnlyNamedPropertyHasErrors(result, erroringProperty);

            return result;
        }

        /// <summary>
        ///     Outputs the validation results.
        /// </summary>
        /// <param name="result">The validation results</param>
        private void Dump(ValidationResult result)
        {
            if (!result.Errors.Any())
            {
                return;
            }

            this.Output.WriteLine($"Found {result.Errors.Count} errors:");

            foreach (ValidationFailure error in result.Errors.OrderBy(keySelector: e => e.PropertyName)
                                                      .ThenBy(keySelector: e => e.ErrorMessage))
            {
                this.Output.WriteLine($" * {error.PropertyName} : {error.ErrorMessage}");
            }
        }

        /// <summary>
        ///     Creates an instance of an object that is valid.
        /// </summary>
        /// <returns>A valid object instance.</returns>
        protected abstract TObject CreateAValidObject();

        /// <summary>
        ///     Tests that all properties in the object pass validation.
        /// </summary>
        protected void TestEverythingValid()
        {
            TObject itemToValidate = this.CreateAValidObject();

            this.Validate(itemToValidate, expectedErrorCount: 0);
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
        /// <param name="erroringProperty">The property expected to have errors.</param>
        protected static void AssertNamedPropertyHasErrors(ValidationResult result, string erroringProperty)
        {
            Assert.True(result.Errors.Any(predicate: e => e.PropertyName == erroringProperty),
                        $"Should have had errors in {erroringProperty}, but not found found errors in {string.Join(separator: ",", result.Errors.Select(selector: e => e.PropertyName).Distinct())}");
        }

        /// <summary>
        ///     Builds the property name from a succession of parts.
        /// </summary>
        /// <param name="parts">The parts.</param>
        /// <returns>The property name.</returns>
        protected static string MakePropertyName(params string[] parts)
        {
            return string.Join(separator: ".", parts);
        }

        /// <summary>
        ///     Checks that everything is valid
        /// </summary>
        [Fact]
        protected abstract void EverythingValid();
    }
}