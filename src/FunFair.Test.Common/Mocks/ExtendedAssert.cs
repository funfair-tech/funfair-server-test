using System.Collections.Generic;
using System.Text.Json;
using Xunit;

namespace FunFair.Test.Common.Mocks
{
    /// <summary>
    ///     Extended assert routines.
    /// </summary>
    public static class ExtendedAssert
    {
        /// <summary>
        ///     Assert if two objects are deep equal by.
        /// </summary>
        /// <param name="expected">The expected object value.</param>
        /// <param name="actual">The actual value.</param>
        public static void DeepEqual<T>(T expected, T actual)
        {
            string expectedString = JsonSerializer.Serialize(expected);
            string actualString = JsonSerializer.Serialize(actual);
            Assert.Equal(expected: expectedString, actual: actualString);
        }

        /// <summary>
        ///     Assert if two objects are deep equal by.
        /// </summary>
        /// <param name="expected">The expected object value.</param>
        /// <param name="actual">The actual value.</param>
        public static void DeepEqual<T>(IReadOnlyList<T> expected, IReadOnlyList<T> actual)
        {
            string expectedString = JsonSerializer.Serialize(expected);
            string actualString = JsonSerializer.Serialize(actual);
            Assert.Equal(expected: expectedString, actual: actualString);
        }
    }
}