using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FunFair.Test.Infrastructure.Mocks;

public sealed class SimpleDefaultModelBindingContext : ModelBindingContext
{
    private const int MAX_MODEL_BINDING_RECURSION_DEPTH = 32;

    private readonly Stack<State> _stack = [];
    private int? _maxModelBindingRecursionDepth;

    private State _state;

    public override ActionContext ActionContext { get; set; } = default!;

    public override string FieldName
    {
        get => this._state.FieldName;
        set => this._state = this._state with { FieldName = value };
    }

    public override object? Model
    {
        get => this._state.Model;
        set => this._state = this._state with { Model = value };
    }

    public override ModelMetadata ModelMetadata
    {
        get => this._state.ModelMetadata;
        set => this._state = this._state with { ModelMetadata = value };
    }

    public override string ModelName
    {
        get => this._state.ModelName;
        set => this._state = this._state with { ModelName = value };
    }

    public override ModelStateDictionary ModelState { get; set; } = default!;

    public override string? BinderModelName
    {
        get => this._state.BinderModelName;
        set => this._state = this._state with { BinderModelName = value };
    }

    public override BindingSource? BindingSource
    {
        get => this._state.BindingSource;
        set => this._state = this._state with { BindingSource = value };
    }

    public override bool IsTopLevelObject
    {
        get => this._state.IsTopLevelObject;
        set => this._state = this._state with { IsTopLevelObject = value };
    }

    public IValueProvider OriginalValueProvider { get; init; } = default!;

    public override IValueProvider ValueProvider
    {
        get => this._state.ValueProvider;
        set => this._state = this._state with { ValueProvider = value };
    }

    public override Func<ModelMetadata, bool>? PropertyFilter
    {
        get => this._state.PropertyFilter;
        set => this._state = this._state with { PropertyFilter = value };
    }

    public override ValidationStateDictionary ValidationState { get; set; } = default!;

    public override ModelBindingResult Result
    {
        get => this._state.Result;
        set => this._state = this._state with { Result = value };
    }

    private int MaxModelBindingRecursionDepth
    {
        get { return this._maxModelBindingRecursionDepth ??= MAX_MODEL_BINDING_RECURSION_DEPTH; }
        init => this._maxModelBindingRecursionDepth = value;
    }

    public static ModelBindingContext CreateBindingContext(
        ActionContext actionContext,
        IValueProvider valueProvider,
        ModelMetadata metadata,
        BindingInfo? bindingInfo,
        string modelName
    )
    {
        string? binderModelName = bindingInfo?.BinderModelName ?? metadata.BinderModelName;
        BindingSource? bindingSource = bindingInfo?.BindingSource ?? metadata.BindingSource;
        IPropertyFilterProvider? propertyFilterProvider =
            bindingInfo?.PropertyFilterProvider ?? metadata.PropertyFilterProvider;

        return new SimpleDefaultModelBindingContext
        {
            ActionContext = actionContext,
            BinderModelName = binderModelName,
            BindingSource = bindingSource,
            PropertyFilter = propertyFilterProvider?.PropertyFilter,
            ValidationState = [],

            // Because this is the top-level context, FieldName and ModelName should be the same.
            FieldName = binderModelName ?? modelName,
            ModelName = binderModelName ?? modelName,

            IsTopLevelObject = true,
            ModelMetadata = metadata,
            ModelState = actionContext.ModelState,
            OriginalValueProvider = valueProvider,
            ValueProvider = FilterValueProvider(valueProvider: valueProvider, bindingSource: bindingSource),
            MaxModelBindingRecursionDepth = MAX_MODEL_BINDING_RECURSION_DEPTH,
        };
    }

    public override NestedScope EnterNestedScope(
        ModelMetadata modelMetadata,
        string fieldName,
        string modelName,
        object? model
    )
    {
        NestedScope scope = this.EnterNestedScope();

        // Only filter if the new BindingSource affects the value providers. Otherwise we want
        // to preserve the current state.
        if (modelMetadata.BindingSource?.IsGreedy == false)
        {
            this.ValueProvider = FilterValueProvider(
                valueProvider: this.OriginalValueProvider,
                bindingSource: modelMetadata.BindingSource
            );
        }

        this.Model = model;
        this.ModelMetadata = modelMetadata;
        this.ModelName = modelName;
        this.FieldName = fieldName;
        this.BinderModelName = modelMetadata.BinderModelName;
        this.BindingSource = modelMetadata.BindingSource;
        this.PropertyFilter = modelMetadata.PropertyFilterProvider?.PropertyFilter;

        this.IsTopLevelObject = false;

        return scope;
    }

    public override NestedScope EnterNestedScope()
    {
        this._stack.Push(this._state);

        // Would this new scope (which isn't in _stack) exceed the allowed recursion depth? That is, has the model
        // binding system already nested MaxModelBindingRecursionDepth binders?
        if (this._stack.Count >= this.MaxModelBindingRecursionDepth)
        {
            throw new InvalidOperationException("Recursion depth exceeded");
        }

        this.Result = default;

        return new(this);
    }

    protected override void ExitNestedScope()
    {
        this._state = this._stack.Pop();
    }

    private static IValueProvider FilterValueProvider(IValueProvider valueProvider, BindingSource? bindingSource)
    {
        return bindingSource?.IsGreedy != false ? BindingSourceValueProvider(valueProvider) : valueProvider;
    }

    private static IValueProvider BindingSourceValueProvider(IValueProvider valueProvider)
    {
        return valueProvider;
    }

    [DebuggerDisplay("{ModelName}: {FieldName}")]
    private readonly record struct State
    {
        public string FieldName { get; init; }

        public object? Model { get; init; }

        public ModelMetadata ModelMetadata { get; init; }

        public string ModelName { get; init; }

        public IValueProvider ValueProvider { get; init; }

        public Func<ModelMetadata, bool>? PropertyFilter { get; init; }

        public string? BinderModelName { get; init; }

        public BindingSource? BindingSource { get; init; }

        public bool IsTopLevelObject { get; init; }

        public ModelBindingResult Result { get; init; }
    }
}
