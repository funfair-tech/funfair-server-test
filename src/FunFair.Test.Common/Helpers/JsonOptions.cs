using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace FunFair.Test.Common.Helpers;

internal static class JsonOptions
{
    [Conditional("NET7_0_OR_GREATER")]
    public static void AddContext(JsonSerializerOptions options, JsonSerializerContext? context)
    {
        if (context is null)
        {
            return;
        }

        if (options.TypeInfoResolver is null)
        {
            options.TypeInfoResolver = context;

            return;
        }

        options.TypeInfoResolver = JsonTypeInfoResolver.Combine(options.TypeInfoResolver, context);
    }
}
