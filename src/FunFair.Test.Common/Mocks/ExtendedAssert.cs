using System.Text.Json;
using Xunit;

namespace FunFair.Test.Common.Mocks
{
    public static class ExtendedAssert
    {
        /// <summary>
        ///     Assert if two objects are deep equal by.
        /// </summary>
        /// <param name="expected">The expected object value.</param>
        /// <param name="actual">The actual value.</param>
        public static void DeepEqual(object expected, object actual)
        {
            string expectedString = JsonSerializer.Serialize(expected);
            string actualString = JsonSerializer.Serialize(actual);
            Assert.Equal(expectedString, actualString);
        }
    }
}