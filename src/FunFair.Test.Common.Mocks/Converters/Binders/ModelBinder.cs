using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FunFair.Test.Common.Mocks.Converters.Binders;

public sealed class ModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        string modelKindName = ModelNames.CreatePropertyModelName(prefix: null, propertyName: bindingContext.ModelName);
        string? modelTypeValue = GetModelTypeValue(bindingContext: bindingContext, modelKindName: modelKindName);

        if (modelTypeValue is null)
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

    private static string? GetModelTypeValue(ModelBindingContext bindingContext, string modelKindName)
    {
        return bindingContext.ValueProvider.GetValue(modelKindName).FirstValue;
    }
}
