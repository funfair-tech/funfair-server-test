using System;
using System.Threading.Tasks;
using FunFair.Test.Common.Mocks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSubstitute;
using Xunit;

namespace FunFair.Test.Common;

public abstract class ModelBinderTestsBase<TBinder, TDataType> : TestBase
    where TBinder : class, IModelBinder
{
    private const string MODEL_NAME = "TestModel";
    private readonly TBinder _binder;
    private readonly IValueProvider _valueProvider;

    protected ModelBinderTestsBase(TBinder binder)
    {
        this._binder = binder ?? throw new ArgumentNullException(nameof(binder));

        this._valueProvider = GetSubstitute<IValueProvider>();
    }

    private void MockValueProvider(string testValue)
    {
        this._valueProvider.GetValue(MODEL_NAME)
            .Returns(new ValueProviderResult(testValue));
    }

    private SimpleDefaultModelBindingContext MockBindingContext()
    {
        return new() { ModelName = MODEL_NAME, ValueProvider = this._valueProvider };
    }

    protected async Task MustConvertAsync(string value, TDataType expected)
    {
        Assert.NotNull(value);

        this.MockValueProvider(value);

        ModelBindingContext bindingContext = this.MockBindingContext();

        await this._binder.BindModelAsync(bindingContext);

        Assert.True(condition: bindingContext.Result.IsModelSet, userMessage: "Should have bound");
        Assert.Equal(expected: expected, actual: bindingContext.Result.Model);
    }

    protected async Task MustNotConvertAsync(string value)
    {
        Assert.NotNull(value);

        this.MockValueProvider(value);

        ModelBindingContext bindingContext = this.MockBindingContext();

        await this._binder.BindModelAsync(bindingContext);

        Assert.False(condition: bindingContext.Result.IsModelSet, userMessage: "Should not have bound");
    }
}