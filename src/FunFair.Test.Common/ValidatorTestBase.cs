using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;
using Xunit;
using Xunit.Abstractions;

namespace FunFair.Test.Common;

public abstract class ValidatorTestBase<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TValidator, TObject> : LoggingTestBase
    where TValidator : AbstractValidator<TObject>, new()
{
    private readonly TValidator _validator;

    protected ValidatorTestBase(ITestOutputHelper output)
        : base(output)
    {
        this._validator = new();
    }

    public ValidationResult Validate(TObject instance)
    {
        ValidationResult result = this._validator.Validate(instance);

        this.Dump(result);

        return result;
    }

    public ValidationResult Validate(TObject instance, int expectedErrorCount)
    {
        ValidationResult result = this.Validate(instance);

        Assert.Equal(expected: expectedErrorCount, actual: result.Errors.Count);

        return result;
    }

    public ValidationResult Validate(TObject instance, int expectedErrorCount, string erroringProperty)
    {
        ValidationResult result = this.Validate(instance: instance, expectedErrorCount: expectedErrorCount);

        AssertOnlyNamedPropertyHasErrors(result: result, erroringProperty: erroringProperty);

        return result;
    }

    public ValidationResult Validate(TObject instance, int expectedErrorCount, params string[] erroringProperties)
    {
        ValidationResult result = this.Validate(instance: instance, expectedErrorCount: expectedErrorCount);

        AssertNamedPropertiesHaveErrors(result: result, erroringProperties: erroringProperties);

        return result;
    }

    protected abstract TObject CreateAValidObject();

    protected void TestEverythingValid()
    {
        TObject itemToValidate = this.CreateAValidObject();

        this.Validate(instance: itemToValidate, expectedErrorCount: 0);
    }

    protected static void AssertOnlyNamedPropertyHasErrors(ValidationResult result, string erroringProperty)
    {
        Assert.True(result.Errors.TrueForAll(e => e.PropertyName == erroringProperty),
                    $"Should only have had errors in {erroringProperty}, but found errors in {string.Join(separator: ',', result.Errors.Select(selector: e => e.PropertyName).Distinct(StringComparer.Ordinal))}");
    }

    [SuppressMessage(category: "ReSharper", checkId: "ParameterOnlyUsedForPreconditionCheck.Local", Justification = "Helper method")]
    protected static void AssertNamedPropertyHasErrors(ValidationResult result, string erroringProperty)
    {
        Assert.True(result.Errors.Exists(e => e.PropertyName == erroringProperty), $"Should have had errors in {erroringProperty}, but not found found errors in {DumpPropertiesInError(result)}");
    }

    protected static void AssertNamedPropertiesHaveErrors(ValidationResult result, params string[] erroringProperties)
    {
        Assert.NotEmpty(erroringProperties);

        bool hasUnexpectedErrors = result.Errors.TrueForAll(error => erroringProperties.Contains(value: error.PropertyName, comparer: StringComparer.Ordinal));
        bool hasAllExpectedErrors = Array.TrueForAll(array: erroringProperties, match: error => result.Errors.Exists(p => p.PropertyName == error));

        Assert.True(condition: hasUnexpectedErrors, $"Should have had errors in {DumpExpectedPropertiesInError(erroringProperties)}, but not found found errors in {DumpPropertiesInError(result)}");

        Assert.True(condition: hasAllExpectedErrors, $"Should have had errors in {DumpExpectedPropertiesInError(erroringProperties)}, but not found found errors in {DumpPropertiesInError(result)}");
    }

    protected static string MakePropertyName(params string[] parts)
    {
        return string.Join(separator: '.', value: parts);
    }

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

    [Fact]
    protected abstract void EverythingValid();
}