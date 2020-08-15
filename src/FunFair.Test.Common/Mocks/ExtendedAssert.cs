using System.Collections.Generic;
using System.Text.Json;

namespace FunFair.Test.Common.Mocks
{
    /// <summary>
    ///     Extended assert routines.
    /// </summary>
    public static class ExtendedAssert
    {
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions {WriteIndented = true};

        /// <summary>
        ///     Assert if two objects are deep equal by.
        /// </summary>
        /// <param name="expected">The expected object value.</param>
        /// <param name="actual">The actual value.</param>
        public static void DeepEqual<T>(T expected, T actual)
        {
            DeepEqual(expected: expected, actual: actual, jsonSerializerOptions: SerializerOptions);
        }

        /// <summary>
        ///     Assert if two objects are deep equal by.
        /// </summary>
        /// <param name="expected">The expected object value.</param>
        /// <param name="actual">The actual value.</param>
        public static void DeepEqual<T>(IReadOnlyList<T> expected, IReadOnlyList<T> actual)
        {
            DeepEqual(expected: expected, actual: actual, jsonSerializerOptions: SerializerOptions);
        }

        /// <summary>
        ///     Assert if two objects are deep equal by.
        /// </summary>
        /// <param name="expected">The expected object value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="jsonSerializerOptions">Serializer Options.</param>
        public static void DeepEqual<T>(T expected, T actual, JsonSerializerOptions jsonSerializerOptions)
        {
            string expectedString = JsonSerializer.Serialize(value: expected, options: jsonSerializerOptions);
            string actualString = JsonSerializer.Serialize(value: actual, options: jsonSerializerOptions);
            Assert.Equal(expected: expectedString, actual: actualString);
        }

        /// <summary>
        ///     Assert if two objects are deep equal by.
        /// </summary>
        /// <param name="expected">The expected object value.</param>
        /// <param name="actual">The actual value.</param>
        /// <param name="jsonSerializerOptions">Serializer Options.</param>
        public static void DeepEqual<T>(IReadOnlyList<T> expected, IReadOnlyList<T> actual, JsonSerializerOptions jsonSerializerOptions)
        {
            string expectedString = JsonSerializer.Serialize(value: expected, options: jsonSerializerOptions);
            string actualString = JsonSerializer.Serialize(value: actual, options: jsonSerializerOptions);
            Assert.Equal(expected: expectedString, actual: actualString);
        }
    }
}