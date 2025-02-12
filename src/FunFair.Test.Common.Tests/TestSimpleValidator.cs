using FluentValidation;
using FunFair.Test.Common.Tests.Mocks;

namespace FunFair.Test.Common.Tests;

public sealed class TestSimpleValidator : AbstractValidator<ExampleObject>
{
    public TestSimpleValidator()
    {
        this.RuleFor(x => x.Name).NotEmpty();
    }
}
