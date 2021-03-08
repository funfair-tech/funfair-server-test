using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FunFair.Test.Common.Helpers;
using Xunit;
using Xunit.Sdk;

namespace FunFair.Test.Common
{
    /// <summary>
    ///     Simple base class for tests.
    /// </summary>
    public abstract class TestBase
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        protected TestBase()
        {
            // Nothing to do here!
            Assert.False(condition: false, userMessage: "Because");
        }

        /// <summary>
        ///     Extracts the result as a task with an optional nullable return.
        /// </summary>
        /// <param name="value">The value to return.</param>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <returns>An optional result</returns>
        protected static Task<T?> FromOptionalResultAsync<T>(T? value)
            where T : class
        {
            return Task.FromResult(value);
        }

        /// <summary>
        ///     Returns a null result for the type.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <returns>A task with a null result.</returns>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        protected static Task<T?> NullResultAsync<T>()
            where T : class
        {
            return FromOptionalResultAsync((T?) null);
        }

        /// <summary>
        ///     Assert that the item is not null and return the non-null value.
        /// </summary>
        /// <param name="value">The value</param>
        /// <typeparam name="T">The item type</typeparam>
        /// <returns>The non null value.</returns>
        protected static T AssertReallyNotNull<T>([NotNull] T? value)
            where T : class
        {
            Assert.NotNull(value);

            if (value == null)
            {
                // Shouldn't need this, but when Assert.NotNull is capable of meaning the same!
                throw new NullException(nameof(value));
            }

            return value;
        }

        /// <summary>
        ///     Suppress that the variable
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        [SuppressMessage(category: "Microsoft.Usage", checkId: "CA1801:ReviewUnusedParameters", Justification = "Needed for Unit Test")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        protected static void UnusedVariable<T>(T value)
        {
            // Marking that the variable is unused.
        }

        /// <summary>
        ///     Verifies that an object is not of the given type or a derived type.
        /// </summary>
        /// <typeparam name="T">The type the object should not be</typeparam>
        /// <param name="obj">The object to be evaluated</param>
        /// <exception cref="Xunit.Sdk.IsAssignableFromException">Thrown when the object is not the given type</exception>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        protected static void IsNotAssignableFrom<T>(object obj)
        {
            IsNotAssignableFrom(typeof(T), obj: obj);
        }

        /// <summary>
        ///     Verifies that an object is not of the given type or a derived type.
        /// </summary>
        /// <param name="expectedType">The type the object should not be</param>
        /// <param name="obj">The object to be evaluated</param>
        /// <exception cref="Xunit.Sdk.IsAssignableFromException">Thrown when the object is not the given type</exception>
        protected static void IsNotAssignableFrom(Type expectedType, object obj)
        {
            if (obj == null || expectedType.GetTypeInfo()
                                           .IsAssignableFrom(obj.GetType()
                                                                .GetTypeInfo()))
            {
                throw new IsAssignableFromException(expected: expectedType, actual: obj);
            }
        }

        /// <summary>
        ///     Formats the value, and checks that it isn't the name
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [SuppressMessage(category: "ReSharper", checkId: "UnusedMember.Global", Justification = "TODO: Review")]
        protected static string FormatValue<T>(T value)
            where T : notnull
        {
            return value.FormatValue();
        }
    }
}

