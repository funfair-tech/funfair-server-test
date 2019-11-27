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
            return this._validator.Validate(instance);
        }

        /// <summary>
        ///     Outputs the validation results.
        /// </summary>
        /// <param name="result">The validation results</param>
        protected void Dump(ValidationResult result)
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
        ///     Checks that everything is valid
        /// </summary>
        protected abstract void EverythingValid();

        /// <summary>
        ///     Tests that all properties in the object pass validation.
        /// </summary>
        protected void TestEverythingValid()
        {
            TObject itemToValidate = this.CreateAValidObject();

            ValidationResult result = this.Validate(itemToValidate);

            this.Dump(result);

            Assert.Equal(expected: 0, result.Errors.Count);
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
        ///     Builds the property name from a succession of parts.
        /// </summary>
        /// <param name="parts">The parts.</param>
        /// <returns>The property name.</returns>
        protected static string MakePropertyName(params string[] parts)
        {
            return string.Join(separator: ".", parts);
        }
    }
}