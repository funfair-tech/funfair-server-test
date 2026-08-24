using FluentValidation;

namespace FunFair.Test.Common.Mocks;

public sealed class TestSimpleValidator : AbstractValidator<ExampleObject>
{
    public TestSimpleValidator()
    {
        this.RuleFor(x => x.Name).NotEmpty();
    }
}
