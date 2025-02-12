using FluentValidation.Results;
using FunFair.Test.Common.Tests.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace FunFair.Test.Common.Tests;

public sealed class ValidatorTestBaseTests : ValidatorTestBase<TestSimpleValidator, ExampleObject>
{
    public ValidatorTestBaseTests(ITestOutputHelper output)
        : base(output) { }

    protected override ExampleObject CreateAValidObject()
    {
        return MockReferenceData.ExampleObject;
    }

    protected override void EverythingValid()
    {
        this.TestEverythingValid();
    }

    [Fact]
    public void NameNullIsInvalid()
    {
        ValidationResult validationResult = this.Validate(new() { Name = null! }, expectedErrorCount: 1, MakePropertyName(nameof(ExampleObject.Name)));

        AssertNamedPropertiesHaveErrors(result: validationResult, nameof(ExampleObject.Name));
        AssertNamedPropertyHasErrors(result: validationResult, nameof(ExampleObject.Name));
        AssertOnlyNamedPropertyHasErrors(result: validationResult, nameof(ExampleObject.Name));
    }

    [Fact]
    public void NameNullIsInvalid2()
    {
        ValidationResult validationResult = this.Validate(new() { Name = null! }, expectedErrorCount: 1, MakePropertyName(nameof(ExampleObject.Name)), MakePropertyName(nameof(ExampleObject.Name)));

        AssertNamedPropertiesHaveErrors(result: validationResult, nameof(ExampleObject.Name));
        AssertNamedPropertyHasErrors(result: validationResult, nameof(ExampleObject.Name));
        AssertOnlyNamedPropertyHasErrors(result: validationResult, nameof(ExampleObject.Name));
    }
}
