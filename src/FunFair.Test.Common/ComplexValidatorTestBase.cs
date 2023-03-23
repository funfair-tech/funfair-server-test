using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Xunit;
using Xunit.Abstractions;

namespace FunFair.Test.Common;

/// <summary>
///     Base class for testing validators with constructor dependency.
/// </summary>
/// <typeparam name="TValidator">The validator to test.</typeparam>
/// <typeparam name="TObject">The object the validator tests</typeparam>
public abstract class ComplexValidatorTestBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TValidator, TObject> : LoggingTestBase
    where TValidator : AbstractValidator<TObject>
{
    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="output">Test output.</param>
    protected ComplexValidatorTestBase(ITestOutputHelper output)
        : base(output)
    {
    }

    /// <summary>
    ///     Validates the object.
    /// </summary>
    /// <param name="instance">The object to validate</param>
    /// <returns>Validation result.</returns>
    public ValidationResult Validate(TObject instance)
    {
        ValidationResult result = this.CreateValidator()
                                      .Validate(instance);

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
    /// <returns>Validation result.</returns>
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
    /// <returns>Validation result.</returns>
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
    ///     Creates an instance of an validator.
    /// </summary>
    /// <returns>A valid object instance.</returns>
    protected abstract TValidator CreateValidator();

    /// <summary>
    ///     Tests that all properties in the object pass validation.
    /// </summary>
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
    [SuppressMessage(category: "ReSharper", checkId: "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Helper method")]
    protected static void AssertOnlyNamedPropertyHasErrors(ValidationResult result, string erroringProperty)
    {
        Assert.True(result.Errors.TrueForAll(e => e.PropertyName == erroringProperty),
                    $"Should only have had errors in {erroringProperty}, but found errors in {string.Join(separator: ",", result.Errors.Select(selector: e => e.PropertyName).Distinct(StringComparer.Ordinal))}");
    }

    /// <summary>
    ///     Check that only the named property has errors.
    /// </summary>
    /// <param name="result">The validation result.</param>
    /// <param name="erroringProperty">The property expected to have errors.</param>
    [SuppressMessage(category: "ReSharper", checkId: "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Helper method")]
    protected static void AssertNamedPropertyHasErrors(ValidationResult result, string erroringProperty)
    {
        Assert.True(result.Errors.Exists(e => e.PropertyName == erroringProperty), $"Should have had errors in {erroringProperty}, but not found found errors in {DumpPropertiesInError(result)}");
    }

    /// <summary>
    ///     Check that only the named property has errors.
    /// </summary>
    /// <param name="result">The validation result.</param>
    /// <param name="erroringProperties">The property expected to have errors.</param>
    protected static void AssertNamedPropertiesHaveErrors(ValidationResult result, params string[] erroringProperties)
    {
        Assert.NotEmpty(erroringProperties);

        bool hasUnexpectedErrors = result.Errors.TrueForAll(error => erroringProperties.Contains(value: error.PropertyName, comparer: StringComparer.Ordinal));
        bool hasAllExpectedErrors = erroringProperties.All(error => result.Errors.Exists(p => p.PropertyName == error));

        Assert.True(condition: hasUnexpectedErrors, $"Should have had errors in {DumpExpectedPropertiesInError(erroringProperties)}, but not found found errors in {DumpPropertiesInError(result)}");

        Assert.True(condition: hasAllExpectedErrors, $"Should have had errors in {DumpExpectedPropertiesInError(erroringProperties)}, but not found found errors in {DumpPropertiesInError(result)}");
    }

    /// <summary>
    ///     Builds the property name from a succession of parts.
    /// </summary>
    /// <param name="parts">The parts.</param>
    /// <returns>The property name.</returns>
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
        if (result.Errors.Count == 0)
        {
            this.Output.WriteLine(message: "Validation Success");

            return;
        }

        this.Output.WriteLine($"Found {result.Errors.Count} errors:");

        foreach (ValidationFailure error in result.Errors.OrderBy(keySelector: e => e.PropertyName, comparer: StringComparer.Ordinal)
                                                  .ThenBy(keySelector: e => e.ErrorMessage, comparer: StringComparer.Ordinal))
        {
            this.Output.WriteLine($" * {error.PropertyName} : {error.ErrorMessage}");
        }
    }

    private static string DumpPropertiesInError(ValidationResult result)
    {
        return string.Join(separator: ", ",
                           result.Errors.Select(selector: e => e.PropertyName)
                                 .Distinct(StringComparer.Ordinal)
                                 .OrderBy(keySelector: x => x, comparer: StringComparer.OrdinalIgnoreCase));
    }

    private static string DumpExpectedPropertiesInError(string[] erroringProperties)
    {
        return string.Join(separator: ", ",
                           erroringProperties.Distinct(StringComparer.Ordinal)
                                             .OrderBy(keySelector: x => x, comparer: StringComparer.OrdinalIgnoreCase));
    }
}