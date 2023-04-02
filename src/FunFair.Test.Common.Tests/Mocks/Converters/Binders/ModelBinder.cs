using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FunFair.Test.Common.Tests.Mocks.Converters.Binders;

public sealed class ModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        string modelKindName = CreatePropertyModelName(prefix: null, propertyName: bindingContext.ModelName);
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

    private static string CreatePropertyModelName(string? prefix, string? propertyName)
    {
        if (string.IsNullOrEmpty(prefix))
        {
            return propertyName ?? string.Empty;
        }

        if (string.IsNullOrEmpty(propertyName))
        {
            return prefix;
        }

        if (propertyName.StartsWith('['))
        {
            // The propertyName might represent an indexer access, in which case combining
            // with a 'dot' would be invalid. This case occurs only when called from ValidationVisitor.
            return prefix + propertyName;
        }

        return prefix + "." + propertyName;
    }
}