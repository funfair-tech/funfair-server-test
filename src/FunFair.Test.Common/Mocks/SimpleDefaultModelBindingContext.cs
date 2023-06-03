using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FunFair.Test.Common.Mocks;

internal sealed class SimpleDefaultModelBindingContext : ModelBindingContext
{
    private const int MAX_MODEL_BINDING_RECURSION_DEPTH = 32;
    private readonly Stack<State> _stack = new();
    private ActionContext _actionContext = default!;
    private int? _maxModelBindingRecursionDepth;
    private ModelStateDictionary _modelState = default!;

    private IValueProvider _originalValueProvider = default!;

    private State _state = new();
    private ValidationStateDictionary _validationState = default!;

    public override ActionContext ActionContext
    {
        get => this._actionContext;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this._actionContext = value;
        }
    }

    public override string FieldName
    {
        get => this._state.FieldName;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this._state.FieldName = value;
        }
    }

    public override object? Model
    {
        get => this._state.Model;
        set => this._state.Model = value;
    }

    public override ModelMetadata ModelMetadata
    {
        get => this._state.ModelMetadata;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this._state.ModelMetadata = value;
        }
    }

    public override string ModelName
    {
        get => this._state.ModelName;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this._state.ModelName = value;
        }
    }

    public override ModelStateDictionary ModelState
    {
        get => this._modelState;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this._modelState = value;
        }
    }

    public override string? BinderModelName
    {
        get => this._state.BinderModelName;
        set => this._state.BinderModelName = value;
    }

    public override BindingSource? BindingSource
    {
        get => this._state.BindingSource;
        set => this._state.BindingSource = value;
    }

    public override bool IsTopLevelObject
    {
        get => this._state.IsTopLevelObject;
        set => this._state.IsTopLevelObject = value;
    }

    public IValueProvider OriginalValueProvider
    {
        get => this._originalValueProvider;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this._originalValueProvider = value;
        }
    }

    public override IValueProvider ValueProvider
    {
        get => this._state.ValueProvider;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this._state.ValueProvider = value;
        }
    }

    public override Func<ModelMetadata, bool>? PropertyFilter
    {
        get => this._state.PropertyFilter;
        set => this._state.PropertyFilter = value;
    }

    public override ValidationStateDictionary ValidationState
    {
        get => this._validationState;
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this._validationState = value;
        }
    }

    public override ModelBindingResult Result
    {
        get => this._state.Result;
        set => this._state.Result = value;
    }

    private int MaxModelBindingRecursionDepth
    {
        get
        {
            return this._maxModelBindingRecursionDepth ??= MAX_MODEL_BINDING_RECURSION_DEPTH;
        }
        set => this._maxModelBindingRecursionDepth = value;
    }

    public static ModelBindingContext CreateBindingContext(ActionContext actionContext, IValueProvider valueProvider, ModelMetadata metadata, BindingInfo? bindingInfo, string modelName)
    {
        if (actionContext is null)
        {
            throw new ArgumentNullException(nameof(actionContext));
        }

        if (valueProvider is null)
        {
            throw new ArgumentNullException(nameof(valueProvider));
        }

        if (metadata is null)
        {
            throw new ArgumentNullException(nameof(metadata));
        }

        if (modelName is null)
        {
            throw new ArgumentNullException(nameof(modelName));
        }

        string? binderModelName = bindingInfo?.BinderModelName ?? metadata.BinderModelName;
        BindingSource? bindingSource = bindingInfo?.BindingSource ?? metadata.BindingSource;
        IPropertyFilterProvider? propertyFilterProvider = bindingInfo?.PropertyFilterProvider ?? metadata.PropertyFilterProvider;

        SimpleDefaultModelBindingContext bindingContext = new()
                                                          {
                                                              ActionContext = actionContext,
                                                              BinderModelName = binderModelName,
                                                              BindingSource = bindingSource,
                                                              PropertyFilter = propertyFilterProvider?.PropertyFilter,
                                                              ValidationState = new(),

                                                              // Because this is the top-level context, FieldName and ModelName should be the same.
                                                              FieldName = binderModelName ?? modelName,
                                                              ModelName = binderModelName ?? modelName,

                                                              //OriginalModelName = binderModelName ?? modelName,
                                                              IsTopLevelObject = true,
                                                              ModelMetadata = metadata,
                                                              ModelState = actionContext.ModelState,
                                                              OriginalValueProvider = valueProvider,
                                                              ValueProvider = FilterValueProvider(valueProvider: valueProvider, bindingSource: bindingSource)
                                                          };

        bindingContext.MaxModelBindingRecursionDepth = MAX_MODEL_BINDING_RECURSION_DEPTH;

        return bindingContext;
    }

    public override NestedScope EnterNestedScope(ModelMetadata modelMetadata, string fieldName, string modelName, object? model)
    {
        if (modelMetadata is null)
        {
            throw new ArgumentNullException(nameof(modelMetadata));
        }

        if (fieldName is null)
        {
            throw new ArgumentNullException(nameof(fieldName));
        }

        if (modelName is null)
        {
            throw new ArgumentNullException(nameof(modelName));
        }

        NestedScope scope = this.EnterNestedScope();

        // Only filter if the new BindingSource affects the value providers. Otherwise we want
        // to preserve the current state.
        if (modelMetadata.BindingSource?.IsGreedy == false)
        {
            this.ValueProvider = FilterValueProvider(valueProvider: this.OriginalValueProvider, bindingSource: modelMetadata.BindingSource);
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
        if (bindingSource?.IsGreedy != false)
        {
            return valueProvider;
        }

        return valueProvider;
    }

    private sealed class State
    {
        public string FieldName { get; set; } = default!;

        public object? Model { get; set; }

        public ModelMetadata ModelMetadata { get; set; } = default!;

        public string ModelName { get; set; } = default!;

        public IValueProvider ValueProvider { get; set; } = default!;

        public Func<ModelMetadata, bool>? PropertyFilter { get; set; }

        public string? BinderModelName { get; set; }

        public BindingSource? BindingSource { get; set; }

        public bool IsTopLevelObject { get; set; }

        public ModelBindingResult Result { get; set; }
    }
}