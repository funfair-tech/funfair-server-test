using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Xunit;

namespace FunFair.Test.Common;

public abstract class ValidatorTestBase<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TValidator,
    TObject
> : ComplexValidatorTestBase<TValidator, TObject>
    where TValidator : AbstractValidator<TObject>, new()
{
    private readonly TValidator _validator;

    protected ValidatorTestBase(ITestOutputHelper output)
        : base(output)
    {
        this._validator = new();
    }

    protected override TValidator CreateValidator()
    {
        return this._validator;
    }

    [Fact]
    protected abstract void EverythingValid();
}
