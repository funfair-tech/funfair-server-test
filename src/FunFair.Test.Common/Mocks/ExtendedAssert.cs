using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace FunFair.Test.Common.Mocks;

public static class ExtendedAssert
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    public static void DeepEqual<T>(T expected, T actual)
    {
        DeepEqual(expected: expected, actual: actual, jsonSerializerOptions: SerializerOptions);
    }

    public static void DeepEqual<T>(IReadOnlyList<T> expected, IReadOnlyList<T> actual)
    {
        DeepEqual(expected: expected, actual: actual, jsonSerializerOptions: SerializerOptions);
    }

    public static void DeepEqual<T>(T expected, T actual, JsonSerializerOptions jsonSerializerOptions)
    {
        string expectedString = JsonSerializer.Serialize(value: expected, options: jsonSerializerOptions);
        string actualString = JsonSerializer.Serialize(value: actual, options: jsonSerializerOptions);
        Assert.Equal(expected: expectedString, actual: actualString);
    }

    public static void DeepEqual<T>(IReadOnlyList<T> expected, IReadOnlyList<T> actual, JsonSerializerOptions jsonSerializerOptions)
    {
        string expectedString = JsonSerializer.Serialize(value: expected, options: jsonSerializerOptions);
        string actualString = JsonSerializer.Serialize(value: actual, options: jsonSerializerOptions);
        Assert.Equal(expected: expectedString, actual: actualString);
    }
}
