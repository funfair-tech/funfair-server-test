using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FunFair.Test.Common.Tests.Mocks.Converters.Binders;

public sealed class ModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        string modelKindName = ModelNames.CreatePropertyModelName(prefix: null, propertyName: bindingContext.ModelName);
        string? modelTypeValue = bindingContext.ValueProvider.GetValue(modelKindName)
                                               .FirstValue;

        if (modelTypeValue == null)
        {
            bindingContext.Result = ModelBindingResult.Failed();

            return Task.CompletedTask;
        }

        if (Enum.TryParse(value: modelTypeValue, ignoreCase: true, out ModelColor found))
        {
            bindingContext.Result = ModelBindingResult.Success(found);

            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Failed();

        return Task.CompletedTask;
    }
}