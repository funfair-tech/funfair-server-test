using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
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
        /// Constructor.
        /// </summary>
        protected TestBase()
        {
            // Nothing to do here!
            Assert.False(false, "Because");
        }

        /// <summary>
        ///     Extracts the result as a task with an optional nullable return.
        /// </summary>
        /// <param name="value">The value to return.</param>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <returns>An optional result</returns>
        protected static Task<T?> FromOptionalResult<T>(T? value)
            where T : class
        {
            return Task.FromResult(value);
        }

        /// <summary>
        ///     Returns a null result for the type.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <returns>A task with a null result.</returns>
        protected static Task<T?> NullResult<T>()
            where T : class
        {
            return FromOptionalResult((T?) null);
        }

        /// <summary>
        /// Assert that the item is not null and return the non-null value.
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
    }
}

