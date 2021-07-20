using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using NSubstitute;
using Xunit;

namespace FunFair.Test.Common
{
    /// <summary>
    ///     Test base class for testing model binders.
    /// </summary>
    /// <typeparam name="TBinder">The binder class.</typeparam>
    /// <typeparam name="TDataType">The data type the binder uses.</typeparam>
    public abstract class ModelBinderTestsBase<TBinder, TDataType> : TestBase
        where TBinder : class, IModelBinder
    {
        private readonly TBinder _binder;
        private readonly IValueProvider _valueProvider;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="binder">The binder to test.</param>
        protected ModelBinderTestsBase(TBinder binder)
        {
            this._binder = binder ?? throw new ArgumentNullException(nameof(binder));

            this._valueProvider = GetSubstitute<IValueProvider>();
        }

        private void MockValueProvider(string testValue)
        {
            this._valueProvider.GetValue("Test")
                .Returns(new ValueProviderResult(testValue));
        }

        private ModelBindingContext MockBindingContext()
        {
            ModelBindingContext bindingContext = new DefaultModelBindingContext {ModelName = "Test", ValueProvider = this._valueProvider};

            return bindingContext;
        }

        /// <summary>
        ///     Checks that the value should convert to the expected model.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="expected">The expected model.</param>
        protected async Task MustConvertAsync(string value, TDataType expected)
        {
            this.MockValueProvider(value);

            ModelBindingContext bindingContext = this.MockBindingContext();

            await this._binder.BindModelAsync(bindingContext);

            Assert.True(condition: bindingContext.Result.IsModelSet, userMessage: "Should have bound");
            Assert.Equal(expected: expected, actual: bindingContext.Result.Model);
        }

        /// <summary>
        ///     Checks that the value does not convert to the expected model.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        protected async Task MustNotConvertAsync(string value)
        {
            this.MockValueProvider(value);

            ModelBindingContext bindingContext = this.MockBindingContext();

            await this._binder.BindModelAsync(bindingContext);

            Assert.False(condition: bindingContext.Result.IsModelSet, userMessage: "Should not have bound");
        }
    }
}