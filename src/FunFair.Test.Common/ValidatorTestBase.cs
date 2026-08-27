using System;
using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using Xunit;
using static FunFair.Test.Common.DispatcherCaseData;

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

    // Single source of truth for the AOT dispatcher case table (see FunFair.Test.Source.Generator's
    // AotTestDispatcherAnalyzer, FTS002); see EquatableObjectTestBase<TObject>.BuildDispatcherCases for why this
    // must stay an ordinary static generic method rather than a [MemberData] provider itself.
    [SuppressMessage(
        category: "Microsoft.Design",
        checkId: "CA1000:Do not declare static members on generic types",
        Justification = "Not a [MemberData] provider itself - a shared helper closed leaf classes call via "
            + "ValidatorTestBase<TValidator, TObject>.BuildDispatcherCases<TSelf>(), avoiding a hand-copied case "
            + "table per leaf"
    )]
    public static (string Name, Action<TSelf> Action)[] BuildDispatcherCases<TSelf>()
        where TSelf : ValidatorTestBase<TValidator, TObject> => [Case<TSelf>(t => t.EverythingValid())];
}
