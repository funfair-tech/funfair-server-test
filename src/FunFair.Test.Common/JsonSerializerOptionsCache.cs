using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using NonBlocking;

namespace FunFair.Test.Common;

// Lives outside the generic test bases so one cache serves every closed generic type; the owner type is part of the
// key so each closed type still gets options built with its own converter.
internal static class JsonSerializerOptionsCache
{
    private static readonly ConcurrentDictionary<
        (Type Owner, JsonSerializerContext Context),
        JsonSerializerOptions
    > Cache = new();

    public static JsonSerializerOptions GetOrAdd(
        Type owner,
        JsonSerializerContext context,
        Func<JsonSerializerContext, JsonSerializerOptions> factory
    )
    {
        (Type Owner, JsonSerializerContext Context) key = (owner, context);

        return Cache.TryGetValue(key: key, out JsonSerializerOptions? options)
            ? options
            : Cache.GetOrAdd(key: key, factory(context));
    }
}
