using System;
using System.Collections.Generic;
using System.Text.Json;
using FunFair.Test.Common.Helpers;
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
        public static void DeepEqual<T>(T expected, T actual)
        {
            string expectedString = JsonSerializer.Serialize(expected);
            string actualString = JsonSerializer.Serialize(actual);
            Assert.Equal(expectedString, actualString);
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
            Assert.Equal(expectedString, actualString);
        }

        /// <summary>
        ///     Assert if two objects are deep equal.
        /// </summary>
        /// <param name="left">The left-most object of the comparison.</param>
        /// <param name="right">The right-most object of the comparison.</param>
        /// <param name="eq">How to do the non-reference equals part of the comparison.</param>
        /// <typeparam name="T">The type of object being compared.</typeparam>
        public static void AreEqual<T>(T? left, T? right, Func<T, T, bool> eq)
            where T : class
        {
            Assert.True(condition: ReferenceObjectHelpers.AreEqual<T>(left, right, eq));
        }

        /// <summary>
        ///     Assert if two objects are deep equal.
        /// </summary>
        /// <param name="left">The left-most object of the comparison.</param>
        /// <param name="right">The right-most object of the comparison.</param>
        /// <param name="compare">How to do the non-reference equals part of the comparison.</param>
        /// <param name="expected">Expected result of comparision</param>
        /// <typeparam name="T">The type of object being compared.</typeparam>
        public static void Compare<T>(T? left, T? right, Func<T, T, int> compare, int expected)
            where T : class
        {
            Assert.Equal(expected: expected, actual: ReferenceObjectHelpers.Compare<T>(left, right, compare));
        }
    }
}