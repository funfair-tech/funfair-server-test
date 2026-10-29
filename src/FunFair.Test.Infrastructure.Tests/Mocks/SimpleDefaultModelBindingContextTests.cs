using FunFair.Test.Common;
using FunFair.Test.Infrastructure.Mocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Xunit;

namespace FunFair.Test.Infrastructure.Tests.Mocks;

public sealed class SimpleDefaultModelBindingContextTests : TestBase
{
    private const string OUTER_MODEL_NAME = "Outer";
    private const string OUTER_MODEL = "outer-model";
    private const string OUTER_RESULT = "outer-result";
    private const string INNER_NAME = "Inner";
    private const string INNER_MODEL = "inner-model";
    private const string INNER_RESULT = "inner-result";

    private readonly ModelBindingContext _context;
    private readonly ModelMetadata _outerMetadata;
    private readonly ModelMetadata _innerMetadata;

    public SimpleDefaultModelBindingContextTests()
    {
        EmptyModelMetadataProvider metadataProvider = new();
        this._outerMetadata = metadataProvider.GetMetadataForType(typeof(ExampleObject));
        this._innerMetadata = metadataProvider.GetMetadataForType(typeof(string));

        ModelStateDictionary modelState = new();
        ActionContext actionContext = new(
            httpContext: new DefaultHttpContext(),
            routeData: new(),
            actionDescriptor: new(),
            modelState: modelState
        );

        this._context = SimpleDefaultModelBindingContext.CreateBindingContext(
            actionContext: actionContext,
            valueProvider: GetSubstitute<IValueProvider>(),
            metadata: this._outerMetadata,
            bindingInfo: null,
            modelName: OUTER_MODEL_NAME
        );

        this._context.Model = OUTER_MODEL;
        this._context.Result = ModelBindingResult.Success(OUTER_RESULT);
    }

    [Fact]
    public void EnterNestedScopeResetsResultInsideScope()
    {
        using (this._context.EnterNestedScope())
        {
            Assert.False(condition: this._context.Result.IsModelSet, userMessage: "Nested scope should start unbound");
        }

        this.AssertOuterStateRestored();
    }

    [Fact]
    public void EnterNestedScopeRestoresOuterStateAfterMutation()
    {
        using (this._context.EnterNestedScope())
        {
            this.MutateInnerState();
        }

        this.AssertOuterStateRestored();
    }

    [Fact]
    public void EnterNestedScopeWithMetadataSetsInnerState()
    {
        using (
            this._context.EnterNestedScope(
                modelMetadata: this._innerMetadata,
                fieldName: INNER_NAME,
                modelName: INNER_NAME,
                model: INNER_MODEL
            )
        )
        {
            this.AssertState(metadata: this._innerMetadata, model: INNER_MODEL, name: INNER_NAME, isTopLevel: false);
            Assert.False(condition: this._context.Result.IsModelSet, userMessage: "Nested scope should start unbound");
        }

        this.AssertOuterStateRestored();
    }

    [Fact]
    public void EnterNestedScopeWithMetadataRestoresOuterStateAfterMutation()
    {
        using (
            this._context.EnterNestedScope(
                modelMetadata: this._innerMetadata,
                fieldName: INNER_NAME,
                modelName: INNER_NAME,
                model: INNER_MODEL
            )
        )
        {
            this.MutateInnerState();
        }

        this.AssertOuterStateRestored();
    }

    private void MutateInnerState()
    {
        this._context.Model = INNER_MODEL;
        this._context.ModelName = INNER_NAME;
        this._context.FieldName = INNER_NAME;
        this._context.Result = ModelBindingResult.Success(INNER_RESULT);
    }

    private void AssertOuterStateRestored()
    {
        this.AssertState(metadata: this._outerMetadata, model: OUTER_MODEL, name: OUTER_MODEL_NAME, isTopLevel: true);
        Assert.True(condition: this._context.Result.IsModelSet, userMessage: "Outer result should be preserved");
        Assert.Equal(expected: OUTER_RESULT, actual: this._context.Result.Model);
    }

    private void AssertState(ModelMetadata metadata, string model, string name, bool isTopLevel)
    {
        Assert.Same(expected: metadata, actual: this._context.ModelMetadata);
        Assert.Equal(expected: model, actual: this._context.Model);
        Assert.Equal(expected: name, actual: this._context.ModelName);
        Assert.Equal(expected: name, actual: this._context.FieldName);
        Assert.Equal(expected: isTopLevel, actual: this._context.IsTopLevelObject);
    }
}
